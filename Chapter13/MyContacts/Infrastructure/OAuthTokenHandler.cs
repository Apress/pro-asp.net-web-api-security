using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth2;

namespace MyContacts.Infrastructure
{
    public class OAuthTokenHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = request.Headers;
                if (headers.Authorization != null)
                {
                    if (headers.Authorization.Scheme.Equals("Bearer"))
                    {
                        string accessToken = request.Headers.Authorization.Parameter;

                        ResourceServer server = new ResourceServer(
                                        new StandardAccessTokenAnalyzer(
                                            (RSACryptoServiceProvider)WebApiApplication.SigningCertificate.PublicKey.Key,
                                            (RSACryptoServiceProvider)WebApiApplication.EncryptionCertificate.PrivateKey
                                        )
                                    );

                        OAuthPrincipal principal = server.GetPrincipal() as OAuthPrincipal;
                        if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                        {
                            var claims = new List<Claim>();

                            foreach (string scope in principal.Roles)
                                claims.Add(new Claim("http://www.my-contacts.com/contacts/OAuth20/claims/scope", scope));

                            claims.Add(new Claim(ClaimTypes.Name, principal.Identity.Name));

                            var identity = new ClaimsIdentity(claims, "Bearer");
                            var newPrincipal = new ClaimsPrincipal(identity);

                            Thread.CurrentPrincipal = newPrincipal;

                            if (HttpContext.Current != null)
                                HttpContext.Current.User = newPrincipal;
                        }
                    }
                }

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(
                                new AuthenticationHeaderValue("Bearer",
                                    "error=\"invalid_token\""));
                }

                return response;
            }
            catch (Exception)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);

                response.Headers.WwwAuthenticate.Add(
                        new AuthenticationHeaderValue("Bearer", "error=\"invalid_token\""));

                return response;
            }
        }
    }
}
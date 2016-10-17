using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MyContacts.Helpers;

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
                        JsonWebToken token = JsonWebToken.Parse(accessToken, EncryptionHelper.Key);

                        var identity = new ClaimsIdentity(token.Claims, "Bearer");
                        var principal = new ClaimsPrincipal(identity);

                        Thread.CurrentPrincipal = principal;

                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
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
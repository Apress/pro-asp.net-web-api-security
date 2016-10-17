using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BasicAuthentication
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private const string SCHEME = "Basic";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = request.Headers;
                if (headers.Authorization != null && SCHEME.Equals(headers.Authorization.Scheme))
                {
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                    string credentials = encoding.GetString(Convert.FromBase64String(
                                                                           headers.Authorization.Parameter));
                    string[] parts = credentials.Split(':');
                    string userId = parts[0].Trim();
                    string password = parts[1].Trim();

                    // TODO - Do authentication of userId and password against your credentials store here
                    if (true)
                    {
                        var claims = new List<Claim>
                        {
                                new Claim(ClaimTypes.Name, userId),
                                new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password)
                        };

                        var principal = new ClaimsPrincipal(
                                                        new[] { new ClaimsIdentity(claims, SCHEME) });

                        Thread.CurrentPrincipal = principal;

                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
                    }
                }

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(
                              new AuthenticationHeaderValue(SCHEME));
                }

                return response;
            }
            catch (Exception)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(SCHEME));

                return response;
            }
        }
    }

}
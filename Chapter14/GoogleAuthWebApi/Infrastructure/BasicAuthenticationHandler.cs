using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoogleAuthWebApi.Infrastructure
{
    public class BasicAuthenticationHandler : DelegatingHandler
    {
        private const string SCHEME = "Basic";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var headers = request.Headers;
            if (headers.Authorization != null && SCHEME.Equals(headers.Authorization.Scheme))
            {
                string credentials = Encoding.UTF8.GetString(
                                                        Convert.FromBase64String(
                                                            headers.Authorization.Parameter));
                string[] parts = credentials.Split(':');
                string userId = parts[0].Trim();
                string password = parts[1].Trim();

                // TODO - Do authentication of userId and password against your credentials store here
                if (true)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userId)
                    };

                    var principal = new ClaimsPrincipal(new[] {
                                            new ClaimsIdentity(claims, SCHEME) });

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
    }
}
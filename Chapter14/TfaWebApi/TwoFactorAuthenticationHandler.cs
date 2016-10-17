using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TfaWebApi
{
    public class TwoFactorAuthenticationHandler : DelegatingHandler
    {
        private const string SCHEME = "Basic";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            X509Certificate2 cert = request.GetClientCertificate();

            if (cert != null)
            {
                X509Chain chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; // Not production strength

                if (chain.Build(cert) && cert.Issuer.Equals("CN=WebApiCA"))
                {
                    var headers = request.Headers;

                    if (headers.Authorization != null && SCHEME.Equals(headers.Authorization.Scheme))
                    {
                        Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                        string credentials = encoding.GetString(
                                                        Convert.FromBase64String(
                                                                    headers.Authorization.Parameter));
                        string[] parts = credentials.Split(':');
                        string userId = parts[0].Trim();
                        string password = parts[1].Trim();

                        string subjectName = cert.Subject.Substring(3); // ignoring CN=

                        // Perform the validation of user Id and password here
                        // For illustration purposes, the factor is considered valid, if user Id and password are the same
                        bool areTwoFactorsValid = !String.IsNullOrWhiteSpace(userId) &&
                                                userId.Equals(password) &&
                                                    userId.Equals(subjectName);

                        if (areTwoFactorsValid)
                        {
                            var claims = new List<Claim>
                            {
                                  new Claim(ClaimTypes.Name, userId)
                            };

                            var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "2FA") });

                            Thread.CurrentPrincipal = principal;

                            if (HttpContext.Current != null)
                                HttpContext.Current.User = principal;
                        }
                    }
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
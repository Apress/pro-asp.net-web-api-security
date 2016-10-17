using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace OwnershipFactorsBasedWebApi
{
    public class AuthenticationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var headers = request.Headers;

            if (headers.Authorization != null && headers.Authorization.Scheme.Equals("Saml"))
            {
                string token = encoding.GetString(
                                            Convert.FromBase64String(headers.Authorization.Parameter));

                using (var stringReader = new StringReader(token))
                {
                    using (var samlReader = XmlReader.Create(stringReader))
                    {
                        var tokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                        SecurityTokenHandlerConfiguration config = tokenHandlers.Configuration;

                        var securityTokens = new List<SecurityToken>()
                        {
                            new X509SecurityToken("CN=RP".ToCertificate())
                        };

                        config.ServiceTokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(
                                                                                securityTokens.AsReadOnly(), false);
                        config.CertificateValidator = X509CertificateValidator.None; // Pay attention
                        config.IssuerTokenResolver = new X509CertificateStoreTokenResolver(StoreName.My,
                                                                                                                               StoreLocation.LocalMachine);
                        config.IssuerNameRegistry = new TrustedIssuerNameRegistry();
                        config.AudienceRestriction.AllowedAudienceUris.Add(new Uri("http://my-server.com"));

                        SecurityToken samlToken = tokenHandlers.ReadToken(samlReader);

                        // Validate the ownership
                        bool isOwnershipValid = false;
                        if (headers.Contains("X-ProofSignature"))
                        {
                            string incomingSignature = headers.GetValues("X-ProofSignature").First();
                            
                            var proofKey = (samlToken.SecurityKeys.First() as InMemorySymmetricSecurityKey).GetSymmetricKey();

                            using (HMACSHA256 hmac = new HMACSHA256(proofKey))
                            {
                                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

                                isOwnershipValid = incomingSignature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal);                
                            }
                        }

                        if (isOwnershipValid)
                        {
                            var identity = tokenHandlers.ValidateToken(samlToken).FirstOrDefault();

                            var principal = new ClaimsPrincipal(new[] { identity });

                            Thread.CurrentPrincipal = principal;

                            if (HttpContext.Current != null)
                                HttpContext.Current.User = principal;
                        }
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
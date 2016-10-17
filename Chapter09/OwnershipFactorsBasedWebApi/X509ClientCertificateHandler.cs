using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OwnershipFactorsBasedWebApi
{
    public class X509ClientCertificateHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var cert = request.GetClientCertificate();
            
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            if (chain.Build(cert) && cert.Issuer.Equals("CN=WebApiCA"))
            {
                var claims = new List<Claim>
                {
                      new Claim(ClaimTypes.Name, cert.Subject.Substring(3)), // ignoring CN=
                };

                var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "X509") });

                Thread.CurrentPrincipal = principal;

                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;

                return await base.SendAsync(request, cancellationToken);
            }

            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }

}
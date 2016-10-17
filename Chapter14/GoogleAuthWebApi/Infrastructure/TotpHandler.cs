using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GoogleAuthWebApi.Helpers;

namespace GoogleAuthWebApi.Infrastructure
{
    public class TotpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;

            IIdentity identity = Thread.CurrentPrincipal.Identity;
            if (request.Headers.Contains("X-TOTP") &&
                                                         identity.IsAuthenticated && !String.IsNullOrWhiteSpace(identity.Name))
            {
                // TODO - Using a hard-coded key for illustration
                // Get the key corresponding to identity.Name from the membership store
                string key = "JBSWY3DPEHPK3PXP";

                if (request.HasValidTotp(key))
                {
                    ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

                    if (identity != null)
                        claimsIdentity.AddClaim(new Claim("http://badri/claims/totp", "true"));
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ClaimsBasedWebApi
{
    public class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
                                    HttpRequestMessage request,
                                    CancellationToken cancellationToken)
        {
            // Pretend this claim comes from a token minted by an STS
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, "jqhuman") }; // User Id of John Q Human

            var id = new ClaimsIdentity(claims, "dummy");
            var principal = new ClaimsPrincipal(new[] { id });

            var config = new IdentityConfiguration();
            var newPrincipal = config.ClaimsAuthenticationManager
                                                .Authenticate(request.RequestUri.ToString(), principal);
            
            Thread.CurrentPrincipal = newPrincipal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = newPrincipal;

            return await base.SendAsync(request, cancellationToken);

        }
    }
}
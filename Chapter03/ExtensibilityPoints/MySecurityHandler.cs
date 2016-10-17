using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ExtensibilityPoints
{
    public class MySecurityHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                                            new ClaimsIdentity(
                                                new List<Claim>() { new Claim(ClaimTypes.Role, "HumanResourceTeamMember") }, 
                                                    "Hard-coded"));

            var response = await base.SendAsync(request, cancellationToken);

            // Inspect and do your stuff with response here

            return response;
        }
    }
}
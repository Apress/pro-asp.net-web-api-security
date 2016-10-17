using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace GoogleAuthWebApi.Infrastructure
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var resource = context.Resource;
            var action = context.Action;

            string resourceName = resource.First(c => c.Type == ClaimTypes.Name).Value;
            string actionName = action.First(c => c.Type == ClaimTypes.Name).Value;

            if (resourceName == "Transfer" && actionName == "Post")
            {
                ClaimsIdentity identity = (context.Principal.Identity as ClaimsIdentity);
                if (!identity.IsAuthenticated)
                    return false;

                var claims = identity.Claims;

                string claimValue = resource.First(c => c.Type == "http://badri/claims/TransferValue").Value;
                decimal trasferValue = Decimal.Parse(claimValue);

                if (trasferValue > 50000M)
                {
                    if (claims.Any(c => c.Type == "http://badri/claims/totp" && Boolean.Parse(c.Value)))
                        return true;
                }
                else
                    return true;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace GoogleAuthWebApi.Helpers
{
    public static class PrincipalHelper
    {
        public static bool CheckAccess(this IPrincipal principal, string resource, string action, IList<Claim> resourceClaims)
        {
            var context = new AuthorizationContext(principal as ClaimsPrincipal, resource, action);
            resourceClaims.ToList().ForEach(c => context.Resource.Add(c));

            var config = new IdentityConfiguration();
            return config.ClaimsAuthorizationManager.CheckAccess(context);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsBasedIdentityConsoleApp
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            string resource = context.Resource.First().Value;
            string action = context.Action.First().Value;

            if (action == "Shoot" && resource == "Gun")
            {
                ClaimsIdentity id = (context.Principal.Identity as ClaimsIdentity);

                if (id.Claims.Any(c => c.Type == ClaimTypes.Role &&
                                                        c.Value.Equals("FireWeaponLicensees")))
                    if (id.Claims.Any(c => c.Type == ClaimTypes.Role &&
                                                        c.Value.Equals("Hunters")))
                        return true;
            }

            return false;
        }
    }

}

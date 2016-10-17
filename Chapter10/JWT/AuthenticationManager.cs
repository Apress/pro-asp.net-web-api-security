using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT
{
    public class AuthenticationManager : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, 
                                                ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal == null)
                throw new SecurityException("Bad Principal!");
            
            ClaimsIdentity identity = (ClaimsIdentity)incomingPrincipal.Identity;
            

            var newClaims = identity.Claims
                                        .SelectMany(c => c.Value.Split(',')
                                            .Select(value => new Claim(c.Type, value))).ToList();

            ClaimsPrincipal newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(newClaims, identity.AuthenticationType));

            return newPrincipal;
        }
    }
}

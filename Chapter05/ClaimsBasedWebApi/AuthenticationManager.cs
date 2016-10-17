using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Web;

namespace ClaimsBasedWebApi
{
    public class AuthenticationManager : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal == null || String.IsNullOrWhiteSpace(incomingPrincipal.Identity.Name))
                throw new SecurityException("Name claim missing");

            // Go to HR database and get the department to which user is assigned to
            // Also, get the role of the user and the country user is based out of
            string department = "Engineering";
            var deptClaim = new Claim("http://badri/claims/department", department);
            var roleClaim = new Claim(ClaimTypes.Role, "Human Resources Manager");
            var countryClaim = new Claim(ClaimTypes.Country, "US");

            ClaimsIdentity identity = (ClaimsIdentity)incomingPrincipal.Identity;
            identity.AddClaim(deptClaim);
            identity.AddClaim(roleClaim);
            identity.AddClaim(countryClaim);

            return incomingPrincipal;
        }
    }
}
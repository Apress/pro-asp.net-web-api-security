using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ClaimsBasedWebApi
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var resource = context.Resource;
            var action = context.Action;

            string resourceName = resource.First(c => c.Type == ClaimTypes.Name).Value;
            string actionName = action.First(c => c.Type == ClaimTypes.Name).Value;

            if (actionName == "Delete" && resourceName == "Employee")
            {
                ClaimsIdentity identity = (context.Principal.Identity as ClaimsIdentity);
                if (!identity.IsAuthenticated)
                    return false;

                var claims = identity.Claims;

                string employeeDepartment = resource.First(c => c.Type == "http://badri/claims/department").Value;
                string employeeCountry = resource.First(c => c.Type == ClaimTypes.Country).Value;

                if (claims.Any(c => c.Type == "http://badri/claims/department" &&
                                                            c.Value.Equals(employeeDepartment)))
                    if (claims.Any(c => c.Type == ClaimTypes.Country &&
                                                                c.Value.Equals(employeeCountry)))
                        if (claims.Any(c => c.Type == ClaimTypes.Role &&
                                                                    c.Value.Equals("Human Resources Manager")))
                            return true;
            }
            return false;
        }
    }

}
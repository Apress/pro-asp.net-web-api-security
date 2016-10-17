using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MyContacts.Infrastructure
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            string resource = context.Resource.First().Value;
            string action = context.Action.First().Value;

            if (action == "Get" && resource == "Contacts")
            {
                ClaimsIdentity id = (context.Principal.Identity as ClaimsIdentity);

                if (!id.IsAuthenticated)
                    return false;

                return (id.Claims
                                  .Any(c => c.Type == "http://www.my-contacts.com/contacts/OAuth20/claims/scope" &&
                                                        c.Value.Equals("Read.Contacts")));
            }

            return false;
        }
    }
}
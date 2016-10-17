using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWT
{
    public class RelyingParty
    {
        // RelyingParty and TokenIssuer share the secret key (symmetric key)
        private string key = "qqO5yXcbijtAdYmS2Otyzeze2XQedqy+Tp37wQ3sgTQ=";

        public void Authenticate(string token)
        {
            try
            {
                SimpleWebToken swt = SimpleWebToken.Parse(token, key);
                Console.WriteLine(swt.ToString());

                // Now, swt.Claims will have the list of claims
                swt.Claims.ToList().ForEach(c => Console.WriteLine("{0} ==> {1}", c.Type, c.Value));

                Thread.CurrentPrincipal = new ClaimsPrincipal(new[] { new ClaimsIdentity(swt.Claims, "SWT") });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Developer")]
        public void TheMethodRequiringAuthZ()
        {
            Console.WriteLine("Remember what uncle Ben said...");
            Console.WriteLine("With great power comes great responsibility");
        }
    }

}

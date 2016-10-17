using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWT
{
    class Program
    {
        static void Main(string[] args)
        {
            // Token issuer
            TokenIssuer issuer = new TokenIssuer();

            // Relying party app
            RelyingParty app = new RelyingParty();

            // A client of the relying party app gets the token
            string token = issuer.GetToken("MyRelyingPartApp", "jqhuman:opensesame");

            // With the token, client now presents the token to Authenticate()
            // and calls the access protected method
            app.Authenticate(token);
            app.TheMethodRequiringAuthZ();
        }

    }
}

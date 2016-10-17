using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT
{
    class Program
    {
        // Main() before the JWE changes commented out below

        //static void Main(string[] args)
        //{
        //    string secretKey = KeyIssuer.GenerateSharedSymmetricKey();

        //    // Token issuer
        //    TokenIssuer issuer = new TokenIssuer();
        //    issuer.ShareKeyOutofBand("MyRelyingPartApp", secretKey);

        //    // Relying Party
        //    RelyingParty app = new RelyingParty();
        //    app.ShareKeyOutofBand(secretKey);

        //    // A client of the relying party app gets the token
        //    string token = issuer.GetToken("MyRelyingPartApp", "opensesame");

        //    // With the token, client now presents the token and calls the method requiring authorization
        //    app.Authenticate(token);
        //    app.TheMethodRequiringAuthZ();
        //}

        static void Main(string[] args)
        {
            string secretKey = KeyIssuer.GenerateSharedSymmetricKey();
            Tuple<string, string> key = KeyIssuer.GenerateAsymmetricKey();

            // Token issuer
            TokenIssuer issuer = new TokenIssuer();
            issuer.ShareKeyOutofBand("MyRelyingPartApp", secretKey);
            issuer.ShareKeyOutofBand("AnotherRelyingPartApp", key.Item1); // Public Key

            // Relying Parties
            RelyingParty app = new RelyingParty();
            app.ShareKeyOutofBand(secretKey);

            RelyingParty anotherApp = new RelyingParty();
            anotherApp.ShareKeyOutofBand(key.Item2); // Private Key

            // A client of the relying party app gets the token
            string token = issuer.GetToken("MyRelyingPartApp", "opensesame");

            // With the token, client now presents the token and calls
            // the method requiring authorization
            app.Authenticate(token);
            app.TheMethodRequiringAuthZ();

            // Use Microsoft JWT Handler
            app.AuthenticateUsingMsftJwt(token);
            app.TheMethodRequiringAuthZ();

            ////////////////////////// Encrypted Token //////////////////////

            // A client of the relying party app gets the token
            token = issuer.GetEncryptedToken("AnotherRelyingPartApp", "opensesame");

            // With the token, client now presents the token and calls
            // the method requiring authorization
            anotherApp.AuthenticateWithEncryptedToken(token);
            anotherApp.TheMethodRequiringAuthZ();
        }

    }

}

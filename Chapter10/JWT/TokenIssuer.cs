using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT
{
    public class TokenIssuer
    {
        private Dictionary<string, string> audienceKeys = new Dictionary<string, string>();

        // This method is called to register a key with the token issuer against an audience or a RP
        public void ShareKeyOutofBand(string audience, string key)
        {
            if (!audienceKeys.ContainsKey(audience))
                audienceKeys.Add(audience, key);
            else
                audienceKeys[audience] = key;
        }

        public string GetToken(string audience, string credentials)
        {
            // Ignoring the crdentials and adding a few claims for illustration
            JsonWebToken token = new JsonWebToken()
            {
                SymmetricKey = audienceKeys[audience],
                Issuer = "TokenIssuer",
                Audience = audience
            };

            token.AddClaim(ClaimTypes.Name, "jqhuman");
            token.AddClaim(ClaimTypes.Role, "Developer");
            token.AddClaim(ClaimTypes.Role, "Admin");

            return token.ToString();
        }

        public string GetEncryptedToken(string audience, string credentials)
        {
            // Ignoring the crdentials and adding a few claims for illustration
            JsonWebEncryptedToken token = new JsonWebEncryptedToken()
            {
                AsymmetricKey = audienceKeys[audience],
                Issuer = "TokenIssuer",
                Audience = audience
            };

            token.AddClaim(ClaimTypes.Name, "jqhuman");
            token.AddClaim(ClaimTypes.Role, "Developer");
            token.AddClaim(ClaimTypes.Role, "Admin");

            return token.ToString();
        }

    }
}

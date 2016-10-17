using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SWT
{
    public class TokenIssuer
    {
        public string GenerateKey(string audience)
        {
            // 256-bit key - we generate and return the key here. In practice, key has to be stored against the
            // audience passed in so that when the same audience asks for a token subsequently, corresponding
            // key can be used.
            using (var provider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyBytes = new Byte[32];
                provider.GetBytes(secretKeyBytes);

                return Convert.ToBase64String(secretKeyBytes);
            }
        }

        public string GetToken(string audience, string credentials)
        {
            // TODO - Authenticate credentials here
            // TODO - Based on the audience passed in, pick the shared key from key store
            // Just hard-coding a key here
            string key = "qqO5yXcbijtAdYmS2Otyzeze2XQedqy+Tp37wQ3sgTQ=";
            SimpleWebToken token = new SimpleWebToken(key) { Issuer = "TokenIssuer" };
            token.AddClaim(ClaimTypes.Name, "jqhuman");
            token.AddClaim(ClaimTypes.Email, "jqhuman@somewhere.world");
            token.AddClaim(ClaimTypes.Role, "Developer");
            token.AddClaim(ClaimTypes.Role, "Administrator");

            return token.ToString();
        }
    }
}

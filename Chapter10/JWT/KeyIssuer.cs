using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWT
{
    public class KeyIssuer
    {
        public static string GenerateSharedSymmetricKey()
        {
            // 256-bit key
            using (var provider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyBytes = new Byte[32];
                provider.GetBytes(secretKeyBytes);

                return Convert.ToBase64String(secretKeyBytes);
            }
        }

        public static Tuple<string, string> GenerateAsymmetricKey()
        {
            string publicKey = String.Empty;
            string privateKey = String.Empty;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }

            return new Tuple<string, string>(publicKey, privateKey);
        }

    }
}

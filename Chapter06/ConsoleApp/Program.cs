using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SymmetricEncryptionDecryption();
            SymmetricSigningVerification();

            AsymmetricEncryptionDecryptionUsingCertificate();
            AsymmetricSigningVerificationUsingCertificate();

            AsymmetricEncryptionDecryptionWithoutCertificate();
            AsymmetricSigningVerificationWithoutCertificate();

            Console.Read();
        }

        private static void SymmetricEncryptionDecryption()
        {
            using (RijndaelManaged provider = new RijndaelManaged())
            {
                byte[] initVector = new byte[provider.BlockSize / 8];
                byte[] key = new byte[provider.KeySize / 8];

                using (var rngProvider = new RNGCryptoServiceProvider())
                {
                    rngProvider.GetBytes(initVector);
                    rngProvider.GetBytes(key);
                }

                // Credit card data that I want to send Alice
                string creditCard = "1234 5678 9012 3456 06/13";
                byte[] clearBytes = Encoding.UTF8.GetBytes(creditCard);

                byte[] foggyBytes = Transform(clearBytes, provider.CreateEncryptor(key, initVector));

                // This is the string that gets sent to Alice
                string encryptedData = Convert.ToBase64String(foggyBytes);
                Console.WriteLine(encryptedData);

                // Since Alice has the key and IV, he can decrypt and read the data
                Console.WriteLine(Encoding.UTF8.GetString(
                                            Transform(foggyBytes, provider.CreateDecryptor(key, initVector))));
            }
        }

        private static void SymmetricSigningVerification()
        {
            // HMAC signing
            byte[] secretKeyBytes = new byte[32];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(secretKeyBytes);
            }

            byte[] dataToBob = Encoding.UTF8.GetBytes("Meet me in the town square");
            string signature = String.Empty;
            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(dataToBob);

                signature = Convert.ToBase64String(signatureBytes);
                Console.WriteLine(signature);
            }

            // Signature verification
            string signatureOfBadri = signature;
            byte[] dataFromBadri = Encoding.UTF8.GetBytes("Meet me in the town square");
            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(dataFromBadri);

                string computedSignature = Convert.ToBase64String(signatureBytes);

                if (computedSignature.Equals(signatureOfBadri, StringComparison.Ordinal))
                    Console.WriteLine("Authentic");
            }
        }

        private static void AsymmetricEncryptionDecryptionUsingCertificate()
        {
            string dataToAlice = "1234 5678 9012 3456 06/13";
            var cert = "CN=Alice".ToCertificate();
            var provider = (RSACryptoServiceProvider)cert.PublicKey.Key;

            byte[] cipherText = provider
                                   .Encrypt(Encoding.UTF8
                                       .GetBytes(dataToAlice), true);

            Console.WriteLine(Convert.ToBase64String(cipherText));

            // What gets sent to Alice is cipherText
            // Alice receives cipherText

            // Alice decrypts the cipherText using his private key
            cert = "CN=Alice".ToCertificate();
            provider = (RSACryptoServiceProvider)cert.PrivateKey;

            Console.WriteLine(
                Encoding.UTF8.GetString(
                    provider.Decrypt(cipherText, true)));
        }

        private static void AsymmetricSigningVerificationUsingCertificate()
        {
            byte[] dataFromBadri = Encoding.UTF8
                                .GetBytes("Meet me in the town square");
            var cert = "CN=Badri".ToCertificate();

            var provider = (RSACryptoServiceProvider)cert.PrivateKey;
            byte[] signatureOfBadri = provider.SignData(dataFromBadri,
                                                 CryptoConfig.MapNameToOID("SHA1"));

            Console.WriteLine(Convert.ToBase64String(signatureOfBadri));

            // What gets sent to Bob are the data and signature
            // dataFromBadri and signatureOfBadri

            // Bob receives my data and signature here
            // dataFromBadri and signatureOfBadri

            // Bob validates the signature using my public key
            cert = "CN=Badri".ToCertificate();
            provider = (RSACryptoServiceProvider)cert.PublicKey.Key;

            if (provider.VerifyData(dataFromBadri,
                                        CryptoConfig.MapNameToOID("SHA1"),
                                        signatureOfBadri))
                Console.WriteLine("Verified");
        }

        private static void AsymmetricEncryptionDecryptionWithoutCertificate()
        {
            string publicKey = String.Empty;
            string privateKey = String.Empty;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }

            byte[] encryptedData = null;
            byte[] secretData = Encoding.UTF8.GetBytes("1234 5678 9012 3456 06/13");

            // Sender's end            
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey); // encrypt using public key
                encryptedData = rsa.Encrypt(secretData, true);
            }

            // Receiver's end
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey); // decrypt using private key
                Console.WriteLine(Encoding.UTF8.GetString(rsa.Decrypt(encryptedData, true)));
            }
        }

        private static void AsymmetricSigningVerificationWithoutCertificate()
        {
            string publicKey = String.Empty;
            string privateKey = String.Empty;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }

            byte[] signature = null;
            byte[] secretData = Encoding.UTF8.GetBytes("1234 5678 9012 3456 06/13");

            // Sender's end
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey); // sign using private key
                signature = rsa.SignData(secretData, "SHA256");
            }

            // Receiver's end
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey); // validate using public key
                Console.Write(rsa.VerifyData(secretData, "SHA256", signature)); // Outputs True, if signature is valid
            }
        }

        private static byte[] Transform(byte[] textBytes, ICryptoTransform transform)
        {
            using (MemoryStream buf = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buf, transform, CryptoStreamMode.Write))
                {
                    stream.Write(textBytes, 0, textBytes.Length);
                    stream.FlushFinalBlock();
                    return buf.ToArray();
                }
            }
        }
    }

    static class CertificateHelper
    {
        public static X509Certificate2 ToCertificate(this string subjectName,
                                                                StoreName name = StoreName.My,
                                                                StoreLocation location = StoreLocation.LocalMachine)
        {
            X509Store store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var cert = store.Certificates.OfType<X509Certificate2>()
                            .FirstOrDefault(c => c.SubjectName.Name.Equals(subjectName,
                                StringComparison.OrdinalIgnoreCase));

                return (cert != null) ? new X509Certificate2(cert) : null;
            }
            finally
            {
                store.Certificates.OfType<X509Certificate2>().ToList().ForEach(c => c.Reset());
                store.Close();
            }
        }
    }
}

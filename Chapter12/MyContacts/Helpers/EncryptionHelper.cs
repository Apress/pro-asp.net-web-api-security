using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MyContacts.Helpers
{
    public static class EncryptionHelper
    {
        private static byte[] initVector = new byte[] { 13, 62, 115, 120, 34, 163, 226, 86 };
        private static byte[] key = new byte[] { 186, 20, 218, 62, 141, 209, 50, 89, 181, 54, 61, 
                                                                          108, 144, 128, 224, 86, 207, 106, 6, 68, 182, 166, 44, 236 };

        public static string Key
        {
            get
            {
                return Convert.ToBase64String(key);
            }
        }

        public static string ToCipherText(this string clearText)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] foggyBytes = Transform(clearBytes, provider.CreateEncryptor(key, initVector));

            return Convert.ToBase64String(foggyBytes);
        }

        public static string ToClearText(this string cipherText)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            byte[] foggyBytes = Convert.FromBase64String(cipherText);

            return Encoding.UTF8.GetString(Transform(foggyBytes, provider.CreateDecryptor(key, initVector)));
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
}
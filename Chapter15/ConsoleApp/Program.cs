using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //////// SHA-256 Hash ////////
            string data = "Supercalifragilisticexpialidocious";
            SHA256 hasher = SHA256.Create();
            byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));

            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Console.WriteLine(hashString);

            //////// SHA-1 Hash with Salt and Key Stretching ////////
            data = "hello";

            byte[] salt = new Byte[32];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(salt);
            }

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(data, salt);
            pbkdf2.IterationCount = 1000;

            hash = pbkdf2.GetBytes(32);
            hashString = Convert.ToBase64String(hash);

            Console.WriteLine(hashString);

            //////// User Credentials Authentication ////////
            string password = String.Empty; // Place holder for the password from database
            string saltString = String.Empty; // Place holder for the salt from database
            string userEnteredPassword = String.Empty; // User input

            // Hard-coding the values
            password = hashString;
            saltString = Convert.ToBase64String(salt);
            userEnteredPassword = "hello";

            pbkdf2 = new Rfc2898DeriveBytes(userEnteredPassword, Convert.FromBase64String(saltString));
            pbkdf2.IterationCount = 1000;
            byte[] computedHash = pbkdf2.GetBytes(32);

            bool isAuthenticCredential = password.Equals(
                                                    Convert.ToBase64String(computedHash),
                                                        StringComparison.Ordinal);
            Console.WriteLine(isAuthenticCredential);
        }
    }
}

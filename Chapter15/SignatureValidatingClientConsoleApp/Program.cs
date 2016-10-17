using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SignatureValidatingClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "foGqiG0GLeY8VGdP2PZoS9aoOB7VjkNaUc549Ac2OCkh2t5rk9wTB0Ebj";
            key += "98I7LGE1mpAkAHXabU/aHTiRhud9A==";

            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri("http://localhost:20759/api/employees/12345");

                string creds = String.Format("{0}:{1}", "badri", "badri");
                byte[] bytes = Encoding.ASCII.GetBytes(creds);
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                client.DefaultRequestHeaders.Authorization = header;

                var result = client.GetAsync(uri).Result;
                string response = result.Content.ReadAsStringAsync().Result;

                string message = String.Format("{0}{1}{2}", uri.ToString(), "GET", response);

                byte[] signature = Encoding.UTF8.GetBytes(message);
                using (HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key)))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);
                    bool isValid = Convert.ToBase64String(signatureBytes)
                                                            .Equals(result.Headers.GetValues("X-Signature").First(),
                                                                StringComparison.Ordinal);
                }
            }
        }
    }
}
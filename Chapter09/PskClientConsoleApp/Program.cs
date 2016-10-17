using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PskClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string publicKey = "DpLMCOihcYI2i6DaMbso9Dzo1miy70G/3+UibTttjLSiJ3cco";
            publicKey += "Kaen3Fecywdf7DrkcfkG3KjeMbZ6djBihD/4A==";

            string privateKey = "W9cE42m+fmBXXvTpYDa2CXIme7DQmk3FcwX0zqR7fmj";
            privateKey += "D6PHHliwdtRb5cOUaxpPyh+3C6Y5Z34uGb2DWD/Awiw==";

            using (HttpClient client = new HttpClient())
            {
                int counter = 33;
                Uri uri = new Uri("http://localhost:54400/api/employees/12345");

                client.DefaultRequestHeaders.Add("X-PSK", publicKey);
                client.DefaultRequestHeaders.Add("X-Counter", String.Format("{0}", counter));

                DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan ts = DateTime.UtcNow - epochStart;
                string stamp = Convert.ToUInt64(ts.TotalSeconds).ToString();
                client.DefaultRequestHeaders.Add("X-Stamp", stamp);

                string data = String.Format("{0}{1}{2}{3}{4}", publicKey, counter, stamp, uri.ToString(), "GET");

                byte[] signature = Encoding.UTF8.GetBytes(data);
                using (HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(privateKey)))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);
                    client.DefaultRequestHeaders.Add("X-Signature", Convert.ToBase64String(signatureBytes));
                }

                var httpMessage = client.GetAsync(uri).Result;
                if(httpMessage.IsSuccessStatusCode)
                    Console.WriteLine(httpMessage.Content.ReadAsStringAsync().Result);   
            }

            Console.Read();
        }
    }

}

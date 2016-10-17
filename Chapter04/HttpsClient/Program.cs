using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HttpsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback =
            (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error) =>
            {
                Console.WriteLine(chain.ChainStatus.First().StatusInformation);
                return true;
            };

            WebClient client = new WebClient();
            var response = client.DownloadString("https://yourserver/api/employees/12345");

        }
    }
}

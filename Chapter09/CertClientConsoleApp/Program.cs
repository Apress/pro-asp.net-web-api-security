using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CertClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (object sender, X509Certificate cer, X509Chain chain, SslPolicyErrors error) =>
                {
                    return true; // This is done because we are using self-signed cert in server side - Not production strength
                };

            var client = WebRequest.Create("https://server.com/api/employees/12345") as HttpWebRequest;
            var cert = new X509Certificate2(File.ReadAllBytes(@"C:\Users\Me\Certs\TestCert.pfx"), "p@ssw0rd!");
            client.ClientCertificates.Add(cert);

            string response = new StreamReader(client.GetResponse().GetResponseStream()).ReadToEnd();

        }
    }
}

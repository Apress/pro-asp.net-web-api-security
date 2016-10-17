using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient client = new WebClient()
            {
                Proxy = new WebProxy("proxy.server.com", 6666)
                {
                    Credentials = CredentialCache.DefaultCredentials
                }
            };

            var response = client.DownloadString("http://localhost.:7077/api/employees/12345");

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CookieClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost.:7077/api/employees/12345";

            CookieWebClient client = new CookieWebClient()
            {
                Proxy = new WebProxy("localhost", 8888) // Fiddler
            };

            Console.WriteLine(client.DownloadString(url)); // Part of this response, cookie is created
            Console.WriteLine(client.DownloadString(url)); // Part of this request, cookie gets sent back to Web API
        }
    }
}

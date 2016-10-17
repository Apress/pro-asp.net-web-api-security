using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Testing Basic Authentication
            using (HttpClient client = new HttpClient())
            {
                string creds = String.Format("{0}:{1}", "badri", "badri");
                byte[] bytes = Encoding.ASCII.GetBytes(creds);
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                client.DefaultRequestHeaders.Authorization = header;

                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("Name", "John Q Human"));

                HttpContent content = new FormUrlEncodedContent(postData);

                string response = String.Empty;
                var responseMessage = client.PostAsync("http://localhost:29724/api/employees/12345", content)
                                        .Result;
                
                if(responseMessage.IsSuccessStatusCode)
                    response = responseMessage.Content.ReadAsStringAsync().Result;
            }

            // Testing Windows Authentication
            // There is no source code included for the server side part of Windows Authentication
            // since setting up Windows Authentication is all about configuration in IIS.
            using (var handler = new HttpClientHandler() { Credentials = CredentialCache.DefaultCredentials })
            {
                using (var httpClient = new HttpClient(handler))
                {

                    var result = httpClient.GetStringAsync("http://localhost/webapi/api/employees/12345").Result;
                }
            }
        }
    }
}

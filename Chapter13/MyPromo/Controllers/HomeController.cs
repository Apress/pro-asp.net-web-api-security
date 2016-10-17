using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth2;
using MyPromo.Models;
using Newtonsoft.Json;

namespace MyPromo.Controllers
{
    public class HomeController : Controller
    {
        const string TOKEN_ENDPOINT = "http://www.my-contacts.com/contacts/OAuth20/Token";
        const string AUTHZ_ENDPOINT = "http://www.my-contacts.com/contacts/OAuth20";

        private readonly string clientId = "0123456789";
        private readonly string clientSecret = "TXVtJ3MgdGhlIHdvcmQhISE=";
        private readonly WebServerClient client;

        private static AuthorizationServerDescription authServer = new AuthorizationServerDescription()
        {
            TokenEndpoint = new Uri(TOKEN_ENDPOINT),
            AuthorizationEndpoint = new Uri(AUTHZ_ENDPOINT),
        };

        public HomeController()
        {
            client = new WebServerClient(authServer, clientId, clientSecret);
        }

        public ActionResult Index(string go)
        {
            if (!String.IsNullOrWhiteSpace(go))
            {
                client.RequestUserAuthorization(new[] { "Read.Contacts" },
                                    new Uri(Url.Action("Exchange", "Home", null, Request.Url.Scheme)));
            }

            return View();
        }

        public ActionResult Exchange()
        {
            var authorization = client.ProcessUserAuthorization();
            if (authorization != null)
            {
                if (authorization.AccessTokenExpirationUtc.HasValue)
                    client.RefreshAuthorization(authorization, TimeSpan.FromSeconds(30));

                string token = authorization.AccessToken;

                string result = String.Empty;
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue
                                                                                                                                ("Bearer", token);
                    var apiResponse = httpClient.GetAsync("http://www.my-contacts.com/contacts/api/contacts")
                                                    .Result;
                    
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        result = apiResponse.Content.ReadAsStringAsync().Result;

                        var contacts = JsonConvert.DeserializeObject<IEnumerable<Contact>>(result);

                        return View(contacts);
                    }
                }
            }

            return View();
        }

        public ActionResult Implicit()
        {
            return View();
        }
    }

}

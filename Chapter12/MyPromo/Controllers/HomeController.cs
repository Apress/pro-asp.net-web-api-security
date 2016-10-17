using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using MyPromo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyPromo.Controllers
{
    public class HomeController : Controller
    {
        string clientId = "0123456789";
        string clientSecret = "TXVtJ3MgdGhlIHdvcmQhISE=";

        public ActionResult Index(string go)
        {
            if (!String.IsNullOrWhiteSpace(go))
            {
                string redirectUri = "http://www.my-promo.com/promo/Home/Exchange";
                string scope = "Read.Contacts";

                string url = "http://www.my-contacts.com/contacts/OAuth20";
                url += "?response_type=code&redirect_uri={0}&client_id={1}&scope={2}";

                url = String.Format(url, redirectUri, clientId, scope);

                return Redirect(url);
            }

            return View();
        }

        public ActionResult Exchange(string code)
        {
            using (HttpClient client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("client_id", clientId));
                postData.Add(new KeyValuePair<string, string>("redirect_uri",
                                                                              "http://www.my-promo.com/promo/Home/Exchange"));
                postData.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                postData.Add(new KeyValuePair<string, string>("code", code));
                postData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));


                HttpContent content = new FormUrlEncodedContent(postData);

                var tokenResponse = client.PostAsync("http://www.my-contacts.com/contacts/OAuth20", content)
                                            .Result;

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var token = tokenResponse.Content.ReadAsStringAsync().Result;

                    string accessToken = (string)(JObject.Parse(token).SelectToken("access_token"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var apiResponse = client.GetAsync("http://www.my-contacts.com/contacts/api/contacts")
                                                .Result;

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        string result = apiResponse.Content.ReadAsStringAsync().Result;

                        var contacts = JsonConvert.DeserializeObject<IEnumerable<Contact>>(result);

                        return View(contacts);
                    }
                }
            }

            return Content("Failed to Exchange the authz code. Hope you find this message informative!");
        }
    }
}
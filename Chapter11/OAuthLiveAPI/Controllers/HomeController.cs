using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OAuthLiveAPI.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<string, string> tokens = new Dictionary<string, string>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Parse()
        {
            return View();
        }

        public ActionResult Login()
        {
            string clientId = "your client id";
            string redirectUri = "http://www.my-server.com/OAuthLiveAPI/Home/Exchange";
            string scope = "wl.signin%20wl.basic%20wl.offline_access";

            string url = "https://login.live.com/oauth20_authorize.srf";
            url += "?response_type=code&redirect_uri={0}&client_id={1}&scope={2}";

            url = String.Format(url, redirectUri, clientId, scope);

            return Redirect(url);
        }

        public ActionResult Exchange(string code)
        {
            string result = String.Empty;

            using (HttpClient client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("client_id", "your client id"));
                postData.Add(new KeyValuePair<string, string>("redirect_uri",
                                                                            "http://www.my-server.com/OAuthLiveAPI/Home/Exchange"));
                postData.Add(new KeyValuePair<string, string>("client_secret", "your client secret"));
                postData.Add(new KeyValuePair<string, string>("code", code)); // retrieved from query string
                postData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

                HttpContent content = new FormUrlEncodedContent(postData);

                var tokenResponse = client.PostAsync("https://login.live.com/oauth20_token.srf", content)
                                            .Result;

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var token = tokenResponse.Content.ReadAsStringAsync().Result;

                    JsonValue value = JsonValue.Parse(token);
                    string accessToken = (string)value["access_token"];
                    string refreshToken = (string)value["refresh_token"];

                    tokens["userId"] = refreshToken;

                    var apiResponse = client
                                        .GetAsync("https://apis.live.net/v5.0/me?access_token=" + accessToken)
                                            .Result;

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        result = apiResponse.Content.ReadAsStringAsync().Result;
                    }
                }
            }

            if (String.IsNullOrEmpty(result))
                return new EmptyResult();
            else
                return Content((string)JsonValue.Parse(result)["name"]);
        }

        public ActionResult Refresh()
        {
            string result = String.Empty;

            using (HttpClient client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("client_id", "your client id"));
                postData.Add(new KeyValuePair<string, string>("redirect_uri", "http://www.my-server.com/OAuthLiveAPI/Home/Exchange"));
                postData.Add(new KeyValuePair<string, string>("client_secret", "your client secret"));
                postData.Add(new KeyValuePair<string, string>("refresh_token", tokens["userId"]));
                postData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));


                HttpContent content = new FormUrlEncodedContent(postData);

                var tokenResponse = client.PostAsync("https://login.live.com/oauth20_token.srf", content)
                                            .Result;

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var token = tokenResponse.Content.ReadAsStringAsync().Result;

                    JsonValue value = JsonValue.Parse(token);
                    string accessToken = (string)value["access_token"];

                    var apiResponse = client.GetAsync("https://apis.live.net/v5.0/me?access_token=" +
                                                                                         accessToken).Result;
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        result = apiResponse.Content.ReadAsStringAsync().Result;
                    }
                }
            }

            if (String.IsNullOrEmpty(result))
                return new EmptyResult();
            else
                return Content("Refreshed " + (string)JsonValue.Parse(result)["name"]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Cookies
{
    public class CookiesHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                CancellationToken cancellationToken)
        {
            // Getting a cookie
            CookieHeaderValue cookie = request.Headers.GetCookies("sesstoken").FirstOrDefault();
            if (cookie != null)
            {
                CookieState cookieState = cookie["sesstoken"];

                string token = cookieState["token"];
                string tokenType = cookieState["token-type"];
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Setting a cookie
            var pairs = new NameValueCollection();
            pairs["token"] = "12345";
            pairs["token-type"] = "general";

            response.Headers.AddCookies(new CookieHeaderValue[]
            {
                new CookieHeaderValue("sesstoken", pairs)
                {
                    Expires = DateTimeOffset.Now.AddSeconds(30),
                    Path = "/"
                }
            });

            return response;
        }
    }
}
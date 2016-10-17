using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace MvcApplication
{
    public class AntiForgeryTokenHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isCsrf = true;

            CookieHeaderValue cookie = request.Headers
                                                .GetCookies(AntiForgeryConfig.CookieName)
                                                    .FirstOrDefault();
            if (cookie != null)
            {
                if (request.Headers.Contains("X-AFT"))
                {
                    try
                    {
                        AntiForgery.Validate(cookie[AntiForgeryConfig.CookieName].Value,
                                                                   request.Headers.GetValues("X-AFT").First());

                        isCsrf = false;
                    }
                    catch (Exception) { }
                }
            }

            if (isCsrf)
            {
                return request.CreateResponse(HttpStatusCode.Forbidden);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
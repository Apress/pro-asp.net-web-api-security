using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HttpsOnlyWebApi
{
    public class HttpsOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var request = context.Request;

            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                var response = request.CreateResponse(HttpStatusCode.Forbidden);
                response.Content = new StringContent("HTTPS Required");
                context.Response = response;
            }
        }
    }

}
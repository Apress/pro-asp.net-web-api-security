using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ExtensibilityPoints
{
    public class MyHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Inspect and do your stuff with request here

            // If you are not happy for a reason, 
            // you can reject the request right here like this
            bool isBadRequest = false;
            if (isBadRequest)
                return request.CreateResponse(HttpStatusCode.BadRequest);

            var response = await base.SendAsync(request, cancellationToken);

            // Inspect and do your stuff with response here

            return response;
        }
    }
}
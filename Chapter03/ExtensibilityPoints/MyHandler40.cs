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
    /// <summary>
    /// .NET 4.0 Message Handler
    /// </summary>
    public class MyHandler40 : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Inspect and do your stuff with request here

            // If you are not happy for a reason, 
            // you can reject the request right here like this
            bool isBadRequest = false;
            if (isBadRequest)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                });
            }

            return base.SendAsync(request, cancellationToken)
                               .ContinueWith((task) =>
                               {
                                   var response = task.Result;

                                   // Inspect and do your stuff with response here
                                   return response;
                               });
        }
    }
}
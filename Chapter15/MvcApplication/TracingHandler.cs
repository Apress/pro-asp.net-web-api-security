using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MvcApplication
{
    public class TracingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpMessageContent requestContent = new HttpMessageContent(request);
            string requestMessage = requestContent.ReadAsStringAsync().Result;

            var response = await base.SendAsync(request, cancellationToken);

            HttpMessageContent responseContent = new HttpMessageContent(response);
            string responseMessage = responseContent.ReadAsStringAsync().Result;

            GlobalConfiguration.Configuration.Services.GetTraceWriter()
                .Trace(request, "MyCategory", System.Web.Http.Tracing.TraceLevel.Info,
                    (t) =>
                    {
                        t.Operation = request.Method.Method;
                        t.Operator = Thread.CurrentPrincipal.Identity.Name;
                        t.Message = requestMessage + Environment.NewLine + responseMessage;
                    });


            return response;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TalentManagerBasic
{
    public class MethodOverrideHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.Headers.Contains("X-HTTP-Method-Override"))
            {
                var method = request.Headers.GetValues("X-HTTP-Method-Override").FirstOrDefault();

                bool isPut = String.Equals(method, "PUT", StringComparison.OrdinalIgnoreCase);
                bool isDelete = String.Equals(method, "DELETE", StringComparison.OrdinalIgnoreCase);

                if (isPut || isDelete)
                {
                    request.Method = new HttpMethod(method);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TalentManager
{
    public class ConcurrencyChecker : ActionFilterAttribute
    {
        private static ConcurrentDictionary<string, EntityTagHeaderValue> etags = new ConcurrentDictionary<string, EntityTagHeaderValue>();

        public override void OnActionExecuting(HttpActionContext context)
        {
            var request = context.Request;

            if (request.Method == HttpMethod.Put)
            {
                var key = request.RequestUri.ToString();

                EntityTagHeaderValue etagFromClient = request.Headers.IfMatch.FirstOrDefault();

                if (etagFromClient != null)
                {
                    EntityTagHeaderValue etag = null;
                    if (etags.TryGetValue(key, out etag) && !etag.Equals(etagFromClient))
                    {
                        context.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var request = context.Request;
            var key = request.RequestUri.ToString();

            EntityTagHeaderValue etag;

            if (!etags.TryGetValue(key, out etag) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
            {
                etag = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
                etags.AddOrUpdate(key, etag, (k, val) => etag);
            }

            context.Response.Headers.ETag = etag;
        }
    }

}
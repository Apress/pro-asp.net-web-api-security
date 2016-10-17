using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TalentManager
{
    public class EnableETag : ActionFilterAttribute
    {
        private static ConcurrentDictionary<string, EntityTagHeaderValue> etags = new
                                                                             ConcurrentDictionary<string, EntityTagHeaderValue>();

        public override void OnActionExecuting(HttpActionContext context)
        {
            var request = context.Request;

            if (request.Method == HttpMethod.Get)
            {
                var key = GetKey(request);

                ICollection<EntityTagHeaderValue> etagsFromClient = request.Headers.IfNoneMatch;

                if (etagsFromClient.Count > 0)
                {
                    EntityTagHeaderValue etag = null;
                    if (etags.TryGetValue(key, out etag) && etagsFromClient.Any(t => t.Tag == etag.Tag))
                    {
                        context.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
                        SetCacheControl(context.Response);
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var request = context.Request;
            var key = GetKey(request);

            EntityTagHeaderValue etag;

            if (!etags.TryGetValue(key, out etag) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
            {
                etag = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
                etags.AddOrUpdate(key, etag, (k, val) => etag);
            }

            context.Response.Headers.ETag = etag;
            SetCacheControl(context.Response);
        }

        private string GetKey(HttpRequestMessage request)
        {
            return request.RequestUri.ToString();
        }

        private void SetCacheControl(HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(6),
                MustRevalidate = true,
                Private = true
            };
        }
    }

}
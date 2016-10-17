using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MvcApplication
{
    public class BruteForceHandler : DelegatingHandler
    {
        private static ConcurrentDictionary<string, List<DateTime>> ipStore = new ConcurrentDictionary<string, List<DateTime>>();

        private static Object listLock = new Object();
        private static IList<string> blackList = new List<string>();

        protected override async Task<HttpResponseMessage> SendAsync(
                                    HttpRequestMessage request,
                                    CancellationToken cancellationToken)
        {

            string key = GetKey(request);

            if (blackList.Contains(key))
            {
                return request.CreateResponse(HttpStatusCode.Forbidden);
            }


            var response = await base.SendAsync(request, cancellationToken);

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated &&
                            response.StatusCode == HttpStatusCode.Unauthorized &&
                                    !String.IsNullOrEmpty(key))
            {
                if (!ipStore.TryAdd(key, new List<DateTime>() { DateTime.Now }))
                {
                    int durationSeconds = 30;
                    List<DateTime> oldLog = null;
                    ipStore.TryGetValue(key, out oldLog);

                    var last30SecondsLog = new List<DateTime>(
                                          oldLog.Where(t => (DateTime.Now - t).TotalSeconds <= durationSeconds));
                    last30SecondsLog.Add(DateTime.Now);

                    ipStore.TryUpdate(key, last30SecondsLog, oldLog);

                    // 15 or more failed requests in the last 30 seconds
                    if (last30SecondsLog.Count > 15)
                    {
                        lock (listLock)
                        {
                            blackList.Add(key);
                        }
                    }
                }
            }

            return response;

        }

        private string GetKey(HttpRequestMessage request)
        {
            if (request.Headers.Authorization != null)
            {
                // Assuming web hosted
                string ip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                // Assuming request Auth header contains credentials in the form of Base64(userID:password)
                string credentials = Encoding.GetEncoding("iso-8859-1")
                                              .GetString(Convert.FromBase64String(request.Headers.Authorization.Parameter));

                string userId = credentials.Split(':')[0];
                return ip + userId;
            }

            return String.Empty;
        }
    }
}
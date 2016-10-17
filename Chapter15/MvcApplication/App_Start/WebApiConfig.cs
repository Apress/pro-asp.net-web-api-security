using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace MvcApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(ITraceWriter), new WebApiTracer());

            config.MessageHandlers.Add(new BruteForceHandler());
            config.MessageHandlers.Add(new BasicAuthHandler());
            
            // Uncomment this only while testing \Home\TestAft
            //config.MessageHandlers.Add(new AntiForgeryTokenHandler());

            config.MessageHandlers.Add(new TracingHandler());
        }
    }
}

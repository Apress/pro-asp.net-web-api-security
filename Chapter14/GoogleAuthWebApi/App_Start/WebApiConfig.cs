using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using GoogleAuthWebApi.Infrastructure;

namespace GoogleAuthWebApi
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

            config.MessageHandlers.Add(new BasicAuthenticationHandler());
            config.MessageHandlers.Add(new TotpHandler()); // Must be after BasicAuthenticationHandler
        }
    }
}

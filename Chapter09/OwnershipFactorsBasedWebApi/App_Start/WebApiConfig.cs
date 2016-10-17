using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OwnershipFactorsBasedWebApi
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

            //config.MessageHandlers.Add(new PskHandler());
            //config.MessageHandlers.Add(new X509ClientCertificateHandler());
            config.MessageHandlers.Add(new AuthenticationHandler());
        }
    }
}

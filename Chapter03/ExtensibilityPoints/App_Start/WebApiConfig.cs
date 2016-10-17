using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace ExtensibilityPoints
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var handler = new MyPremiumSecurityHandler()
            {
                InnerHandler = new MyOtherPremiumSecurityHandler()
                {
                    InnerHandler = new HttpControllerDispatcher(config)
                }
            };

            config.Routes.MapHttpRoute(
                name: "premiumApi",
                routeTemplate: "premium/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: handler
            );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new MySecurityHandler());
            config.MessageHandlers.Add(new MyHandler());
        }
    }
}

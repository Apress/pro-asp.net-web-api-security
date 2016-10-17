using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TalentManagerSecured
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

            // Uncomment the line below, if you want to remove authorization element from web.config
            //config.Filters.Add(new AuthorizeAttribute());
        }
    }
}

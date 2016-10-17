using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyContacts.Filters
{
    public class LoginRequired : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpCookie cookie = context.HttpContext.Request.Cookies[".contacts"];
            if (cookie != null)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(cookie.Value), null);
            }
            else
            {
                context.Result = new RedirectToRouteResult(
                                            new RouteValueDictionary(
                                                new { Action = "Login", Controller = "Home" }));
            }
        }
    }

}
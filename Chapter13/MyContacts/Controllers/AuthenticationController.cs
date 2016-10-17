using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyContacts.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string userId, string password, string returnUrl)
        {
            bool isAuthentic = !String.IsNullOrWhiteSpace(userId) && userId.Equals(password);

            if (isAuthentic)
                FormsAuthentication.SetAuthCookie(userId, false);

            return Redirect(returnUrl ?? Url.Action("Index", "Home"));
        }
    }
}

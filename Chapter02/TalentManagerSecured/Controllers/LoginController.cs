using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TalentManagerSecured.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(string returnurl)
        {
            return View(); // present the login page to the user
        }

        // Login page gets posted to this action method
        [HttpPost]
        public ActionResult Index(string userId, string password)
        {
            if (userId.Equals(password)) // dumb check for illustration
            {
                // Create the ticket and stuff it in a cookie
                FormsAuthentication.SetAuthCookie("Badri", false);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }

}

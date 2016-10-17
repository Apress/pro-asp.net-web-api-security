using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MyContacts.Filters;
using MyContacts.Models;

namespace MyContacts.Controllers
{
    public class HomeController : Controller
    {
        [LoginRequired]
        public ActionResult Index()
        {
            string owner = Thread.CurrentPrincipal.Identity.Name;

            return View(Contact.GenerateContacts()
                                    .Where(c => c.Owner == owner));
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userId, string password)
        {
            if (!String.IsNullOrWhiteSpace(userId))
            {
                if (userId.Equals(password)) // consider this an authentic user
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userId), null);

                    // create cookie with userId
                    // Encrypt this - never use clear text cookies
                    Response.Cookies.Add(new HttpCookie(".contacts", userId));

                    return RedirectToAction("Index");
                }
            }

            return View();
        }
    }
}

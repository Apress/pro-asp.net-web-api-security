﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class BadController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}

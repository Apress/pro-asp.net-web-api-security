using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyContacts.Helpers;

namespace MyContacts
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static X509Certificate2 signingCertificate = "CN=AuthSrv".ToCertificate();
        private static X509Certificate2 encryptionCertificate = "CN=ResSrv".ToCertificate();

        public static X509Certificate2 SigningCertificate
        {
            get
            {
                return signingCertificate;
            }
        }

        public static X509Certificate2 EncryptionCertificate
        {
            get
            {
                return encryptionCertificate;
            }
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
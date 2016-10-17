using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using GoogleAuthWebApi.Helpers;

namespace GoogleAuthWebApi.Infrastructure
{
    public class TwoFactorAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext context)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated && !String.IsNullOrWhiteSpace(identity.Name))
            {
                // TODO - Using a hard-coded key for illustration
                // Get the key corresponding to identity.Name from the membership store
                string key = "JBSWY3DPEHPK3PXP";

                if (context.Request.HasValidTotp(key))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "TOTP code required"
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using MyContacts.Infrastructure;
using MyContacts.Infrastructure.Store;
using MyContacts.Models;

namespace MyContacts.Controllers
{
    public class OAuth20Controller : Controller
    {
        private readonly AuthorizationServer server = new AuthorizationServer(new ServerHost());

        [Authorize]
        public ActionResult Index()
        {
            var request = this.server.ReadAuthorizationRequest();
            if (request == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "Bad request");

            var model = new AuthorizationRequest
            {
                ClientApp = DataStore.Instance.Clients
                                .First(c => c.ClientIdentifier == request.ClientIdentifier).Name,
                Scope = request.Scope,
                Request = request,
            };

            return View(model);
        }

        [Authorize, HttpPost]
        public ActionResult Index(string userApproval)
        {
            var request = this.server.ReadAuthorizationRequest();
            if (request == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "Bad request");

            if (!String.IsNullOrWhiteSpace(userApproval))
            {
                // Record the authorization against the client and user
                DataStore.Instance.Clients
                    .First(c => c.ClientIdentifier == request.ClientIdentifier)
                        .ClientAuthorizations.Add(
                            new ClientAuthorization
                            {
                                Scope = request.Scope,
                                UserId = User.Identity.Name,
                                IssueDate = DateTime.UtcNow
                            });

                var response = this.server.PrepareApproveAuthorizationRequest(request, User.Identity.Name);

                return this.server.Channel.PrepareResponse(response).AsActionResult();
            }

            return View();
        }

        public ActionResult Token()
        {
            return this.server.HandleTokenRequest(this.Request).AsActionResult();
        }
    } 
}

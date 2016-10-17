using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyContacts.Models;
using MyContacts.Helpers;
using MyContacts.Infrastructure;
using System.Security.Claims;

namespace MyContacts.Controllers
{
    public class OAuth20Controller : Controller
    {
        private Dictionary<string, Tuple<string, string>> applicationRegistry = new Dictionary<string,
                                                                                                                                          Tuple<string, string>>();
        private static ConcurrentDictionary<Guid, Tuple<string, string, string>> codesIssued = new
                                                                                  ConcurrentDictionary<Guid, Tuple<string, string, string>>();

        public OAuth20Controller()
        {
            // client id, client secret and domain
            applicationRegistry.Add("0123456789",
                                 new Tuple<string, string>("TXVtJ3MgdGhlIHdvcmQhISE=", "http://www.my-promo.com"));
        }

        [HttpGet]
        public ActionResult Index(AuthzCodeRequest request)
        {
            bool isRequestValid = false;
            if (ModelState.IsValid)
            {
                if (applicationRegistry.ContainsKey(request.client_id))
                {
                    var registryInfo = applicationRegistry[request.client_id];
                    if (request.redirect_uri.StartsWith(registryInfo.Item2))
                    {
                        if (request.scope.Equals("Read.Contacts"))
                        {
                            if (request.response_type.Equals("code"))
                                isRequestValid = true;
                        }
                    }
                }
            }

            if (!isRequestValid)
                return RedirectToAction("Error");

            ViewBag.CipherText = String.Format("{0}|{1}", request.redirect_uri, request.scope)
                                        .ToCipherText();

            return View(); // shows login screen that posts to Authenticate
        }

        [HttpPost]
        public ActionResult Authenticate(string userId, string password, bool isOkayToShare, string CipherText)
        {
            // Authenticate
            bool isAuthentic = !String.IsNullOrWhiteSpace(userId) && userId.Equals(password);

            if (!isAuthentic)
                return RedirectToAction("Error");

            if (isOkayToShare)
            {
                var tokens = CipherText.ToClearText().Split('|');
                string uri = tokens[0];
                string scope = tokens[1];

                Guid code = Guid.NewGuid();
                codesIssued.TryAdd(code, new Tuple<string, string, string>(userId, uri, scope));

                uri += "?code=" + code.ToString();

                return Redirect(uri);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult Index(TokenRequest request)
        {
            bool isRequestValid = false;
            Tuple<string, string, string> grantData = null;

            // All required inputs are present
            if (ModelState.IsValid)
            {
                // client Id is in the registry
                if (applicationRegistry.ContainsKey(request.client_id))
                {
                    var registryInfo = applicationRegistry[request.client_id];
                    // client secret is the registy against the client id
                    if (request.client_secret.Equals(registryInfo.Item1))
                    {
                        // grant type is correct
                        if (request.grant_type.Equals("authorization_code"))
                        {
                            Guid code = Guid.Parse(request.code);
                            // we have issued the code, since it is present in our list of codes issued
                            if (codesIssued.TryGetValue(code, out grantData))
                            {
                                // Token request is for the same redirect URI for which we previously issued the code
                                if (grantData != null && request.redirect_uri.Equals(grantData.Item2))
                                {
                                    // all is well - remove the authz code from our list
                                    isRequestValid = true;
                                    codesIssued.TryRemove(code, out grantData);
                                }
                            }
                        }
                    }
                }
            }

            if (isRequestValid)
            {
                JsonWebToken token = new JsonWebToken()
                {
                    SymmetricKey = EncryptionHelper.Key,
                    Issuer = "http://www.my-contacts.com/contacts/OAuth20",
                    Audience = "http://www.my-promo.com/promo/Home"
                };

                token.AddClaim(ClaimTypes.Name, grantData.Item1);
                token.AddClaim("http://www.my-contacts.com/contacts/OAuth20/claims/scope", grantData.Item3);

                return Json(new { access_token = token.ToString() });
            }

            // OAuth 2.0 spec requires the right code to be returned
            // For example, if authorization code is invalid, invalid_grant must be returned
            // I'm just returning 'invalid_request' as a catch all thing, just for brevity
            return Json(new { error = "invalid_request" });
        }


    }

}

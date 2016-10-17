using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using GoogleAuthWebApi.Helpers;

namespace GoogleAuthWebApi.Controllers
{
    public class TransfersController : ApiController
    {
        //[TwoFactor]
        public HttpResponseMessage Post(AccountTransfer transfer)
        {
            // Based on id, retrieve employee details and create the list of resource claims
            var transferClaims = new List<Claim>()
                                    {
                                        new Claim("http://badri/claims/TransferValue", transfer.Amount.ToString())
                                    };

            if (User.CheckAccess("Transfer", "Post", transferClaims))
            {
                //repository.MakeTransfer(transfer);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "TOTP code required" };

        }
    }

    public class AccountTransfer
    {
        public decimal Amount { get; set; }
    }
}
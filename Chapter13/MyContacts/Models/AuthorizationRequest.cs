using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth2.Messages;

namespace MyContacts.Models
{
    public class AuthorizationRequest
    {
        public string ClientApp { get; set; }

        public HashSet<string> Scope { get; set; }

        public EndUserAuthorizationRequest Request { get; set; }
    }
}
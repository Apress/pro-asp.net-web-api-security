using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyContacts.Infrastructure.Store
{
    public class ClientAuthorization
    {
        public DateTime IssueDate { get; set; }

        public string UserId { get; set; }

        public HashSet<string> Scope { get; set; }

        public Nullable<DateTime> ExpirationDateUtc { get; set; }
    }

}
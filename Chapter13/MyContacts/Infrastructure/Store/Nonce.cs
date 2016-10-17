using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyContacts.Infrastructure.Store
{
    public class Nonce
    {
        public string Context { get; set; }

        public string Code { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
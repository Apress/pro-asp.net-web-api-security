using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;

namespace MyContacts.Infrastructure.Store
{
    public class NonceStore : INonceStore
    {
        private IList<Nonce> nonces = new List<Nonce>();

        public bool StoreNonce(string context, string nonce, DateTime timestampUtc)
        {
            if (nonces.Any(n => n.Context == context && n.Code == nonce && n.Timestamp == timestampUtc))
                return false; // Possibly a replay attack, return false
            else
            {
                Nonce newNonce = new Nonce { Context = context, Code = nonce, Timestamp = timestampUtc };

                nonces.Add(newNonce);
                return true;
            }
        }
    }

}
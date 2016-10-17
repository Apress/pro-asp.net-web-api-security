using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;

namespace MyContacts.Infrastructure.Store
{
    public class SymmetricCryptoKey : CryptoKey
    {
        public SymmetricCryptoKey(byte[] key, DateTime expiresUtc) : base(key, expiresUtc) { }

        public SymmetricCryptoKey(CryptoKey key) : base(key.Key, key.ExpiresUtc) { }

        public string Bucket { get; set; }

        public string Handle { get; set; }
    }
}
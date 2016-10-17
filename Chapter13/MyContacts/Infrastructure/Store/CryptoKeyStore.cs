using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;

namespace MyContacts.Infrastructure.Store
{
    public class CryptoKeyStore : ICryptoKeyStore
    {
        private IList<SymmetricCryptoKey> cryptoKeys = new List<SymmetricCryptoKey>();

        public CryptoKey GetKey(string bucket, string handle)
        {
            return cryptoKeys.Where(k => k.Bucket == bucket && k.Handle == handle).FirstOrDefault();
        }

        public IEnumerable<KeyValuePair<string, CryptoKey>> GetKeys(string bucket)
        {
            return cryptoKeys.Where(k => k.Bucket == bucket)
                        .OrderByDescending(o => o.ExpiresUtc)
                            .Select(kvp => new KeyValuePair<string, CryptoKey>(kvp.Handle, kvp));
        }

        public void RemoveKey(string bucket, string handle)
        {
            var key = cryptoKeys.FirstOrDefault(k => k.Bucket == bucket && k.Handle == handle);
            if (key != null)
                cryptoKeys.Remove(key);
        }

        public void StoreKey(string bucket, string handle, CryptoKey key)
        {
            cryptoKeys.Add(new SymmetricCryptoKey(key) { Bucket = bucket, Handle = handle });
        }
    }
}
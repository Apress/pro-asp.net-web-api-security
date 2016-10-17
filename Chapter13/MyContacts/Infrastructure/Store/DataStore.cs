using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;

namespace MyContacts.Infrastructure.Store
{
    public class DataStore
    {
        private static DataStore store = null;

        static DataStore()
        {
            store = new DataStore();
        }

        private DataStore()
        {
            this.Clients = new List<Client>();
            this.CryptoKeyStore = new CryptoKeyStore();
            this.NonceStore = new NonceStore();

            this.Clients.Add(
                        new Client()
                        {
                            Name = "My Promotion Manager",
                            ClientIdentifier = "0123456789",
                            ClientSecret = "TXVtJ3MgdGhlIHdvcmQhISE=",
                            DefaultCallback = new Uri("http://www.my-promo.com/promo"),
                            ClientType = ClientType.Confidential
                        });
        }

        public static DataStore Instance
        {
            get
            {
                return store;
            }
        }


        public IList<Client> Clients { get; set; }

        public ICryptoKeyStore CryptoKeyStore { get; set; }
        public INonceStore NonceStore { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.OAuth2.ChannelElements;
using DotNetOpenAuth.OAuth2.Messages;
using MyContacts.Infrastructure.Store;

namespace MyContacts.Infrastructure
{
    public class ServerHost : IAuthorizationServerHost
    {
        public AccessTokenResult CreateAccessToken(IAccessTokenRequest accessTokenRequestMessage)
        {
            var accessToken = new AuthorizationServerAccessToken();
            accessToken.Lifetime = TimeSpan.FromMinutes(2);

            // Using the certificate of our one and only resource server blindly
            accessToken.ResourceServerEncryptionKey =
                                         (RSACryptoServiceProvider)WebApiApplication.EncryptionCertificate.PublicKey.Key;

            accessToken.AccessTokenSigningKey =
                                                    (RSACryptoServiceProvider)WebApiApplication.SigningCertificate.PrivateKey;

            var result = new AccessTokenResult(accessToken);
            return result;
        }

        public ICryptoKeyStore CryptoKeyStore
        {
            get { return DataStore.Instance.CryptoKeyStore; }
        }

        public INonceStore NonceStore
        {
            get { return DataStore.Instance.NonceStore; }
        }

        public IClientDescription GetClient(string clientIdentifier)
        {
            return DataStore.Instance.Clients.First(c => c.ClientIdentifier == clientIdentifier);
        }

        public bool IsAuthorizationValid(IAuthorizationDescription authorization)
        {
            var client = DataStore.Instance.Clients
                                        .First(c => c.ClientIdentifier == authorization.ClientIdentifier);

            var authorizations = client.ClientAuthorizations
                                        .Where(a => a.UserId == authorization.User &&
                                                        a.IssueDate <= authorization.UtcIssued.AddSeconds(1) &&
                                                            (!a.ExpirationDateUtc.HasValue ||
                                                                a.ExpirationDateUtc.Value >= DateTime.UtcNow));
            if (!authorizations.Any()) // No authorizations
                return false;

            var grantedScopes = new HashSet<string>();
            authorizations.ToList().ForEach(a => grantedScopes.UnionWith(a.Scope));

            return authorization.Scope.IsSubsetOf(grantedScopes);
        }

        public bool TryAuthorizeClientCredentialsGrant(IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }

        public bool TryAuthorizeResourceOwnerCredentialGrant(string userName, string password,
                                                              IAccessTokenRequest accessRequest, out string canonicalUserName)
        {
            throw new NotImplementedException();
        }

        public AutomatedAuthorizationCheckResponse CheckAuthorizeClientCredentialsGrant(IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }

        public AutomatedUserAuthorizationCheckResponse CheckAuthorizeResourceOwnerCredentialGrant(string userName, string password, IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }
    }
}
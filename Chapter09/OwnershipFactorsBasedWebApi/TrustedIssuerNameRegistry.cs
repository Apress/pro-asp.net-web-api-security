using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace OwnershipFactorsBasedWebApi
{
    public class TrustedIssuerNameRegistry : IssuerNameRegistry
    {
        private const string THE_ONLY_TRUSTED_ISSUER = "CN=MySTS";

        public override string GetIssuerName(SecurityToken securityToken)
        {
            using (X509SecurityToken x509Token = (X509SecurityToken)securityToken)
            {
                string name = x509Token.Certificate.SubjectName.Name;

                return name.Equals(THE_ONLY_TRUSTED_ISSUER) ? name : String.Empty;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MySTS
{
    public class MySecurityTokenService : SecurityTokenService
    {
        public MySecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration) { }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
    {
        if (request.AppliesTo == null)
                throw new InvalidRequestException("Specify RP in AppliesTo");

        if (!request.AppliesTo.Uri.Equals(new Uri("http://my-server.com")))
        {
            Console.WriteLine("Invalid Relying party Address ");
            throw new InvalidRequestException("Invalid Relying party Address ");
        }

        var encryptingCredentials = new X509EncryptingCredentials("CN=RP".ToCertificate());

        Scope scope = new Scope(request.AppliesTo.Uri.AbsoluteUri,
            SecurityTokenServiceConfiguration.SigningCredentials,
            encryptingCredentials);

        return scope;
    }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal,
                                                                                RequestSecurityToken request, Scope scope)
        {
            string userName = principal.Identity.Name;
            string authenticationType = principal.Identity.AuthenticationType;

            var outputIdentity = new ClaimsIdentity(authenticationType);

            Claim nameClaim = new Claim(System.IdentityModel.Claims.ClaimTypes.Name, userName);
            Claim emailClaim = new Claim(ClaimTypes.Email, userName + "@somewhere.com");

            outputIdentity.AddClaim(nameClaim);
            outputIdentity.AddClaim(emailClaim);

            return outputIdentity;
        }
    }

}

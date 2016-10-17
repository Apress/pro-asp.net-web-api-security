using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens.JWT;

namespace JWT
{
    public class RelyingParty
    {
        private string secretKey = String.Empty;

        public void ShareKeyOutofBand(string key)
        {
            this.secretKey = key;
        }

        public void Authenticate(string token)
        {
            JsonWebToken jwt = null;

            try
            {
                jwt = JsonWebToken.Parse(token, this.secretKey);

                // Now, swt.Claims will have the list of claims
                jwt.Claims.ToList().ForEach(c => Console.WriteLine("{0} ==> {1}", c.Type, c.Value));

                Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "JWT"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AuthenticateUsingMsftJwt(string token)
        {
            try
            {
                // Use JWTSecurityTokenHandler to validate the JWT token
                JWTSecurityTokenHandler tokenHandler = new JWTSecurityTokenHandler();

                TokenValidationParameters parms = new TokenValidationParameters()
                {
                    AllowedAudience = "MyRelyingPartApp",
                    ValidIssuers = new List<string>() { "TokenIssuer" },
                    SigningToken = new BinarySecretSecurityToken(Convert.FromBase64String(this.secretKey))
                };

                var config = new IdentityConfiguration();
                Thread.CurrentPrincipal = config
                                            .ClaimsAuthenticationManager
                                                .Authenticate("TheMethodRequiringAuthZ", 
                                                                    tokenHandler.ValidateToken(token, parms));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AuthenticateWithEncryptedToken(string token)
        {
            JsonWebEncryptedToken jwt = null;

            try
            {
                jwt = JsonWebEncryptedToken.Parse(token, this.secretKey);

                // Now, swt.Claims will have the list of claims
                jwt.Claims.ToList().ForEach(c => Console.WriteLine("{0} ==> {1}", c.Type, c.Value));

                Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "JWT"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "Developer")]
        public void TheMethodRequiringAuthZ()
        {
            Console.WriteLine("With great power comes great responsibility - Uncle Ben");
        }
    }
}

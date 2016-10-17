using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace MySTS
{
    class Program
    {
        static void Main(string[] args)
        {
            SigningCredentials signingCreds = new X509SigningCredentials("CN=MySTS".ToCertificate());

            SecurityTokenServiceConfiguration config =
                new SecurityTokenServiceConfiguration("http://MySTS", signingCreds);

            config.SecurityTokenHandlers.AddOrReplace(new CustomUsernameTokenHandler());
            config.SecurityTokenService = typeof(MySecurityTokenService);

            // Create the WS-Trust service host with our STS configuration
            var host = new WSTrustServiceHost(config, new Uri("http://localhost:6000/MySTS"));

            try
            {
                host.Open();
                Console.WriteLine("STS is ready to issue tokens… Press ENTER to shutdown");
                Console.ReadLine();
                host.Close();
            }
            finally
            {
                if (host.State != CommunicationState.Faulted)
                    host.Close();
                else
                    host.Abort();
            }
        }
    }
}

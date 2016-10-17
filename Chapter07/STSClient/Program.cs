using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace STSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = GetToken();
            Console.WriteLine(token);
        }
        
        private static string GetToken()
        {
            var binding = new WS2007HttpBinding(SecurityMode.Message);

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Message.NegotiateServiceCredential = true;
            binding.Security.Message.EstablishSecurityContext = false;

            var address = new EndpointAddress(new Uri(@"http://localhost:6000/MySTS"),
                                                            new DnsEndpointIdentity("MySTS"));

            WSTrustChannelFactory factory = new WSTrustChannelFactory(binding, address);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                                                                                                                       X509CertificateValidationMode.None;
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            factory.Credentials.UserName.UserName = "jqhuman";
            factory.Credentials.UserName.Password = "jqhuman"; // got to be same as user name in our example

            WSTrustChannel channel = (WSTrustChannel)factory.CreateChannel();

            var request = new RequestSecurityToken(System.IdentityModel.Protocols.WSTrust.RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://my-server.com")
            };

            RequestSecurityTokenResponse response = null;
            var token = channel.Issue(request, out response) as GenericXmlSecurityToken;

            // Proof key
            var proofKey = Convert.ToBase64String(response.RequestedProofToken.ProtectedKey.GetKeyBytes());

            return token.TokenXml.OuterXml;
        }
    }
}

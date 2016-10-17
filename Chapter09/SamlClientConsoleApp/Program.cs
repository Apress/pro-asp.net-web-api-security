using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace SamlClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Tuple<string, byte[]> token = GetToken();

            string saml = token.Item1;
            byte[] proofKey = token.Item2;

            using (HttpClient client = new HttpClient())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(saml);
                var header = new AuthenticationHeaderValue("Saml", Convert.ToBase64String(bytes));
                client.DefaultRequestHeaders.Authorization = header;

                using (HMACSHA256 hmac = new HMACSHA256(proofKey))
                {
                    byte[] signatureBytes = hmac.ComputeHash(bytes);
                    client.DefaultRequestHeaders.Add("X-ProofSignature", Convert.ToBase64String(signatureBytes));
                }

                var httpMessage = client.GetAsync("http://localhost:54400/api/employees/12345")
                                        .Result;
                if (httpMessage.IsSuccessStatusCode)
                    Console.WriteLine(httpMessage.Content.ReadAsStringAsync().Result);
            }
        }

        private static Tuple<string, byte[]> GetToken()
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

            var proofKey = response.RequestedProofToken.ProtectedKey.GetKeyBytes();

            return new Tuple<string,byte[]>(token.TokenXml.OuterXml, proofKey);
        }

        private static string GetTokenFromAdfs20()
        {

            var binding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Message.NegotiateServiceCredential = true;
            binding.Security.Message.EstablishSecurityContext = false;

            var address = new EndpointAddress(
                                              new Uri(@"https://yourserver.com/adfs/services/trust/13/usernamemixed"));

            WSTrustChannelFactory factory = new WSTrustChannelFactory(binding, address);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.UserName.UserName = "You Active Directory User Id";
            factory.Credentials.UserName.Password = "Corresponding password";

            WSTrustChannel channel = (WSTrustChannel)factory.CreateChannel();

            var request = new RequestSecurityToken(System.IdentityModel.Protocols.WSTrust.RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("https://relyingparty"),
                KeyType = KeyTypes.Bearer
            };

            RequestSecurityTokenResponse response = null;
            var token = channel.Issue(request, out response) as GenericXmlSecurityToken;

            return token.TokenXml.OuterXml;
        }


    }
}

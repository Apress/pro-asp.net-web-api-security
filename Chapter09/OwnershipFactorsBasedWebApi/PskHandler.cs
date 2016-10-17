using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OwnershipFactorsBasedWebApi
{
    public class PskHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string privateKey = "W9cE42m+fmBXXvTpYDa2CXIme7DQmk3FcwX0zqR7fmj";
            privateKey += "D6PHHliwdtRb5cOUaxpPyh+3C6Y5Z34uGb2DWD/Awiw==";

            var headers = request.Headers;

            if (headers.Contains("X-PSK") && headers.Contains("X-Counter") &&
                    headers.Contains("X-Stamp") && headers.Contains("X-Signature"))
            {
                string publicKey = headers.GetValues("X-PSK").First();
                string counter = headers.GetValues("X-Counter").First();
                ulong stamp = Convert.ToUInt64(headers.GetValues("X-Stamp").First());
                string incomingSignature = headers.GetValues("X-Signature").First();

                string data = String.Format("{0}{1}{2}{3}{4}", publicKey, counter, stamp,
                                                                            request.RequestUri.ToString(),
                                                                            request.Method.Method);

                byte[] signature = Encoding.UTF8.GetBytes(data);
                using (HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(privateKey)))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);
                    if (incomingSignature.Equals(
                                           Convert.ToBase64String(signatureBytes), StringComparison.Ordinal))
                    {
                        DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
                        TimeSpan ts = DateTime.UtcNow - epochStart;

                        if (Convert.ToUInt64(ts.TotalSeconds) - stamp <= 3)
                            return await base.SendAsync(request, cancellationToken);
                    }
                }
            }

            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }

}
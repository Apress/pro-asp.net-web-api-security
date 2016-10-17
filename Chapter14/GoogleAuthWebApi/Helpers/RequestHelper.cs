using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using GoogleAuthWebApi.Infrastructure;

namespace GoogleAuthWebApi.Helpers
{
    public static class RequestHelper
    {
        public static bool HasValidTotp(this HttpRequestMessage request, string key)
        {
            if (request.Headers.Contains("X-TOTP"))
            {
                string totp = request.Headers.GetValues("X-TOTP").First();

                // We check against the past, current and the future TOTP
                if (Totp.GetPastCurrentFutureOtp(key).Any(p => p.Equals(totp)))
                    return true;
            }

            return false;

        }
    }
}
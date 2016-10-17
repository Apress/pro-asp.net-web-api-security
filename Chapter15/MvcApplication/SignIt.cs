using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace MvcApplication
{
    public class SignIt : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            // 256-bit shared key - hard-coded here only for the purpose of this example
            string key = "foGqiG0GLeY8VGdP2PZoS9aoOB7VjkNaUc549Ac2OCkh2t5rk";
            key += "9wTB0Ebj98I7LGE1mpAkAHXabU/aHTiRhud9A==";

            string response = context.Response.Content.ReadAsStringAsync().Result;

            if (!String.IsNullOrWhiteSpace(response))
            {
                string data = String.Format("{0}{1}{2}", context.Request.RequestUri.ToString(),
                                                            context.Request.Method,
                                                                response);

                byte[] bytes = Encoding.UTF8.GetBytes(data);
                using (HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key)))
                {
                    string signature = Convert.ToBase64String(hmac.ComputeHash(bytes));
                    context.Response.Headers.Add("X-Signature", signature);
                }
            }
        }
    }
}
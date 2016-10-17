using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web;
using System.Web.Http.Filters;

namespace TalentManagerBasic
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private IDictionary<Type, HttpStatusCode> map = null;

        public ExceptionFilter()
        {
            map = new Dictionary<Type, HttpStatusCode>();
            map.Add(typeof(ArgumentException), HttpStatusCode.BadRequest);
            map.Add(typeof(SecurityException), HttpStatusCode.Unauthorized);
            map.Add(typeof(NotImplementedException), HttpStatusCode.NotImplemented);
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (!(context.Exception is HttpException))
            {
                context.Response = new HttpResponseMessage(map[context.Exception.GetType()])
                {
                    Content = new StringContent(context.Exception.Message)
                };
            }
        }
    }
}
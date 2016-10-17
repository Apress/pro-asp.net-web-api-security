using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ClaimsBasedWebApi.Controllers
{
    public class EmployeesController : ApiController
    {
        public HttpResponseMessage Delete(int id)
        {
            // Based on id, retrieve employee details and create the list of resource claims
            var employeeClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Country, "US"),
                new Claim("http://badri/claims/department", "Engineering")
            };

            if (User.CheckAccess("Employee", "Delete", employeeClaims))
            {
                //repository.Remove(id);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            else
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
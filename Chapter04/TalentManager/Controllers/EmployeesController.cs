using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TalentManager.Models;

namespace TalentManager.Controllers
{
    public class EmployeesController : ApiController
    {
        public HttpResponseMessage GetAllEmployees()
        {
            var employees = new Employee[]
                {
                        new Employee()
                        {
                                Id = 12345,
                                Name = "John Q Law",
                                Department = "Enforcement"
                        },
                        new Employee()
                        {
                                Id = 45678,
                                Name = "Jane Q Taxpayer",
                                Department = "Revenue"
                        }
                };

            var response = Request.CreateResponse<IEnumerable<Employee>>
                                                             (HttpStatusCode.OK, employees);

            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(6),
                MustRevalidate = true,
                Private = true
            };

            return response;
        }

        [EnableETag]
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id, Name = "John Q Human", Department = "Enforcement"
            };
        }

        [ConcurrencyChecker]
        public void Put(Employee employee)
        {
        }
    }
}
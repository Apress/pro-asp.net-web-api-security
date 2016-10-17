using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Tracing;
using MvcApplication.Models;

namespace MvcApplication.Controllers
{
    public class EmployeesController : ApiController
    {
        [Authorize]
        public IEnumerable<Employee> Get()
        {
            Configuration.Services.GetTraceWriter().Trace(Request,
                                                            "MyCategory",
                                                            TraceLevel.Info,
                                                            (t) =>
                                                            {
                                                                t.Operation = Request.Method.Method;
                                                                t.Operator = User.Identity.Name;
                                                                t.Message = "Get Employees";
                                                            });

            return new Employee[]
            {
                new Employee() { Id = 12345, FirstName = "John", LastName = "Human" },
                new Employee() { Id = 67890, FirstName = "Jane", LastName = "Public" }
            };
        }

        [Authorize]
        [SignIt]
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id,
                FirstName = "John",
                LastName = "Human"
            };
        }

        public void Post(Employee emp)
        {
            // repository.Save(emp);
        }

        public void Put(Employee emp)
        {
            // repository.Save(emp);
        }

    }
}
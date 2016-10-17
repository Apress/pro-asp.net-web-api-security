using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployeesWebAPI.Controllers
{
    public class EmployeesController : ApiController
    {
        public Employee Get(string id)
        {
            return new Employee() { Id = id, Name = "John Q Human" };
        }
    }

    public class Employee
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // other members
    }
}
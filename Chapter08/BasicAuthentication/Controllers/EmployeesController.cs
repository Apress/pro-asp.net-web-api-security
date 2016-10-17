using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BasicAuthentication.Models;

namespace BasicAuthentication.Controllers
{
    public class EmployeesController : ApiController
    {
        [Authorize]
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id,
                Name = "John Q Human"
            };
        }

        [Authorize]
        public Employee Post(Employee human)
        {
            human.Id = 12345;
            return human;
        }
    }
}
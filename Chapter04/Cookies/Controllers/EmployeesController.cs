using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cookies.Models;

namespace Cookies.Controllers
{
    public class EmployeesController : ApiController
    {
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id, Name = "John  Q Human"
            };
        }
    }
}

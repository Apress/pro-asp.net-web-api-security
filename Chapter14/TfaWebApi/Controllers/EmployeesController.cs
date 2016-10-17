using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TfaWebApi.Models;

namespace TfaWebApi.Controllers
{
    public class EmployeesController : ApiController
    {
        // Uncomment Authorize attribute after configuring client certificate
        //[Authorize]
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id,
                Name = "John Q Human"
            };
        }
    }
}
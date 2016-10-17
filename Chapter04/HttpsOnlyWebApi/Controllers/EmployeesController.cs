using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpsOnlyWebApi.Models;

namespace HttpsOnlyWebApi.Controllers
{
    public class EmployeesController : ApiController
    {
        [HttpsOnly]
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id,
                Name = "John  Q Human"
            };
        }
    }
}
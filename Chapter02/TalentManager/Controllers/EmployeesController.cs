using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TalentManager.Models;

namespace TalentManager.Controllers
{
    public class EmployeesController : ApiController
    {
        public Employee Get(int id)
        {
            return new Employee()
            {
                Id = id,
                Name = "John Q Law",
                Department = "Enforcement"
            };
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return new Employee[]
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
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExtensibilityPoints.Models;

namespace ExtensibilityPoints.Controllers
{
    [Authorize(Roles = "HumanResourceTeamMember")]
    public class EmployeesController : ApiController
    {
        public IEnumerable<Employee> Get()
        {
                return new List<Employee>()
                {
                        new Employee() { Id = 12345, Name = "John Q Human" },
                        new Employee() { Id = 23456, Name = "Jane Q Public" }
                };
        }

        public Employee Post(Employee human)
        {
            // Add employee to the system
            human.Id = 12345; // Id produced as a result of adding the employee to data store
            return human;
        }

        [Authorize(Roles = "ManagementTeamMember")]
        public void Delete(int id)
        {
            // Delete employee from the system
        }

        public void Put(Employee employee)
        {
            // Update employee in the system
        }
    }
}
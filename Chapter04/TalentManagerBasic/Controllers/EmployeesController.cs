using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TalentManagerBasic.Models;

namespace TalentManagerBasic.Controllers
{
    public class EmployeesController : ApiController
    {
        private readonly IRepository repository = null;

        public EmployeesController()
        {
            // Poor man's dependency injection - for illustration purposes only
            this.repository = new Repository();
        }

        [ExceptionFilter]
        public IEnumerable<Employee> Get(string department)
        {
            if (!String.Equals(department, "HR", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Bad Department");

            return new List<Employee>()
            {
                new Employee() { Id = 12345, Name = "John Q Human" }
            };
        }

        public Employee Get(int id)
        {
            if (id > 999999)
            {
                throw new HttpResponseException
                (
                        new HttpResponseMessage()
                        {
                            Content = new StringContent("Invalid employee id"),
                            StatusCode = HttpStatusCode.BadRequest
                        }
                    );
            }

            return new Employee() { Id = 12345, Name = "John Q Human" };
        }

        public HttpResponseMessage Post(Employee human)
        {
            // store the human object using repository pattern
            // Repository returns an employee object, which contains an id
            Employee employee = repository.Add(human);

            var response = Request.CreateResponse<Employee>(HttpStatusCode.Created, employee);

            string uri = Url.Link("DefaultApi", new { id = employee.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        // Returns 204 – No Content
        public void Put(Employee human)
        {
        }

        // Returns 204 – No Content
        public void Delete(int id)
        {
        }

        // Handles TRACE method
        [AcceptVerbs("TRACE")]
        public void Echo()
        {
        }
    }
}
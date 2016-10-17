using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cors.Models;

namespace Cors.Controllers
{
    public class EmployeesController : ApiController
    {
        public HttpResponseMessage Get(int id)
        {
            var employee = new Employee()
            {
                Id = id,
                Name = "John Q Human",
                Department = "Enforcement"
            };

            var response = Request.CreateResponse<Employee>(HttpStatusCode.OK, employee);
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            return response;
        }

        public HttpResponseMessage Put(Employee employee)
        {
            // Update logic goes here

            var response = Request.CreateResponse<Employee>(HttpStatusCode.OK, employee);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }

        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "PUT");
            return response;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfEmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        public Employee GetEmployee(string id)
        {
            return new Employee() { Id = id, Name = "John Q Human" };
        }

    }
}

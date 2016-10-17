using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MvcApplication.Models;

namespace MvcApplication
{
    public class DataAccess
    {
        // Implementation vulnerable to SQL injection
        public static Employee Get(string id)
        {
            string connectionString = "Put your connection string here";
            string sql = "SELECT * FROM employee WHERE employee_id = " + id; // id is straight from the URI
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    // use the reader returned by cmd.ExecuteReader(); to create Employee object
                }
            }

            return new Employee();
        }

        // Better implementation using parameters
        public static Employee GetUsingParameters(string id)
        {
            string connectionString = "Put your connection string here";
            string sql = "SELECT * FROM employee WHERE employee_id = @ID;";
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters["@ID"].Value = id; // from the URI after validation

                    connection.Open();
                    // use the reader returned by cmd.ExecuteReader(); to create Employee object
                }
            }

            return new Employee();
        }
    }
}
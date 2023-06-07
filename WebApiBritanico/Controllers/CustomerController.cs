using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace WebApiBritanico.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CustomerController : ControllerBase
    {

        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _configuration;

        public CustomerController(ILogger<CustomerController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<Customer> Search(string? name)
        {
            List<Customer> result = new List<Customer>();
            string connectionString = _configuration.GetConnectionString("SqlConnection");
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string cmdText = @"select 1";
                    using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
                    {
                        sqlCommand.ExecuteScalar();
                        result.Add(new Customer() { CustomerID = 1 });
                    }
                    //string cmdText = @"select top 50 CustomerID, Title, FirstName, MiddleName, LastName, CompanyName from SalesLT.Customer
                    //                where @name IS NULL OR FirstName LIKE '%' + @name + '%'";
                    //using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
                    //{
                    //    sqlCommand.Parameters.Clear();
                    //    sqlCommand.Parameters.Add(new SqlParameter("@name", name == null ? DBNull.Value : name));
                    //    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            Customer customer = new Customer();
                    //            customer.CustomerID = reader.GetInt32(0);
                    //            customer.Title = reader.IsDBNull(1) ? "": reader.GetString(1);
                    //            customer.FirstName = reader.GetString(2);
                    //            customer.MiddleName = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    //            customer.LastName = reader.GetString(4);
                    //            customer.CompanyName = reader.GetString(5);
                    //            result.Add(customer);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception(string.Format("SqlConnection: {0}, name: {1}", connectionString, name), ex);
            }
            return result;
        }

        [HttpGet("list")]
        public IEnumerable<Customer>Getall()
        {
            List<Customer> result = new List<Customer>();
            for (int i = 0; i < 10; i++)
            {
                Customer customer = new Customer();
                customer.CustomerID = i + 1;
                customer.Title = "Titulo " + i;
                customer.FirstName = "FirstName " + i;
                customer.MiddleName = "MiddleName " + i;
                customer.LastName = "LastName " + i;
                customer.CompanyName = "CompanyName " + i;
                result.Add(customer);
            }
            
            return result;
        }
    }
}
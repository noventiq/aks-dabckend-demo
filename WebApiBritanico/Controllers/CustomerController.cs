using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net.Http.Headers;
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
        public Conectividad Search(string? name)
        {
            Conectividad item = new Conectividad();
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
                        item.Satisfactorio = true;
                        item.Mensaje = "Correcto";
                        item.Detalle = "";
                        item.ValorOriginal = connectionString;
                        item.ValorCambiado = connectionString;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SqlConnection: {0}, name: {1}, exception: {2}", connectionString, name, ex.ToString());
                item.Satisfactorio = true;
                item.Mensaje = "Error";
                item.Detalle = string.Format("SqlConnection: {0}, name: {1}, exception: {2}", connectionString, name, ex.ToString());
                item.ValorOriginal = connectionString;
                item.ValorCambiado = connectionString;
            }

            return item;
        }

        [HttpGet("list")]
        public IEnumerable<Customer> Getall()
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
    
        [HttpGet("ping")]
        public async Task<string> GetPing([FromQuery]string url)
        {
            string detalle = string.Empty;
            try
            {
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("api/v1/ping");
                    if (response.IsSuccessStatusCode)
                    {
                        detalle = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        detalle = "Internal server Error";
                    }
                }
            }
            catch (System.Exception ex)
            {
                detalle = ex.ToString();
            }
            return detalle;
        }

        [HttpGet("pong")]
        public async Task<string> GetPong([FromQuery]string url)
        {
            string detalle = string.Empty;
            try
            {
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("api/v1/pong");
                    if (response.IsSuccessStatusCode)
                    {
                        detalle = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        detalle = "Internal server Error";
                    }
                }
            }
            catch (System.Exception ex)
            {
                detalle = ex.ToString();
            }
            return detalle;
        }
    
    }
}
using Microsoft.AspNetCore.Mvc;

namespace WebApiPing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public string Get([FromQuery]string? ipSource)
        {
            if(ipSource == null)
            {
                return "hola " + ipSource + ", saludos desde ping";
            }

            return "hola desde ping";
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace WebApiPong.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PongController : ControllerBase
    {
        private readonly ILogger<PongController> _logger;

        public PongController(ILogger<PongController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public string Get([FromQuery]string? ipSource)
        {
            if (ipSource != null)
            {
                return "hola " + ipSource + ", saludos desde pong";
            }

            return "hola desde pong";
        }
    }
}
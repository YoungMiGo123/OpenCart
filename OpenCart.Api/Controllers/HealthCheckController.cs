using Microsoft.AspNetCore.Mvc;

namespace OpenCart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        [Route("Health")]
        public IActionResult Health()
        {
            return Ok("Success");
        }
    }
}

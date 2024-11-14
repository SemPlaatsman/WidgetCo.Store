using Microsoft.AspNetCore.Mvc;

namespace WidgetCo.Store.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Test controller endpoint hit");
            return Ok("Hello from API Controller!");
        }

        [HttpGet("detail")]
        public IActionResult GetDetail()
        {
            return Ok(new { message = "Detail endpoint", timestamp = DateTime.UtcNow });
        }
    }
}
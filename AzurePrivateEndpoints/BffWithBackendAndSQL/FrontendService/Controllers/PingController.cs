using Microsoft.AspNetCore.Mvc;

namespace FrontendService.Controllers
{
    [Route("ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get() => "pong";
    }
}

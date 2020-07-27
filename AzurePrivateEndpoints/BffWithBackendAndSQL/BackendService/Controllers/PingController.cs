using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [Route("ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get() => "pong";
    }
}

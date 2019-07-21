using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiDemo.Controllers
{
    /// <summary>
    /// Throws unhandled exception for logging demo
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FaultyController : ControllerBase
    {
        private readonly ILogger logger;

        public FaultyController(ILogger<FaultyController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Throws an exception, used to demo logging and error responses
        /// </summary>
        /// <exception cref="ApplicationException">Always throws this exception</exception>
        /// <returns>Nothing because of exception</returns>
        [HttpGet]
        public string GetSomething()
        {
            logger.LogWarning("I think, something is going to fail...");

            throw new ApplicationException("Something bad happened.");
        }
    }
}
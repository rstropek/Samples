using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private long counter = 0;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            if (++counter % 10 == 0)
            {
                throw new ApplicationException("Something bad happened");
            }

            if (counter % 10 < 5)
            {
                await Task.Delay(new Random().Next(250, 300));
            }

            

            return new string[] { "value1", "value2" };
        }
    }
}

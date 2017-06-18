using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProtectedWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [Authorize("read:data")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize("write:data")]
        [HttpDelete]
        public void Delete()
        {
            // Here we would delete something
            return;
        }
    }
}

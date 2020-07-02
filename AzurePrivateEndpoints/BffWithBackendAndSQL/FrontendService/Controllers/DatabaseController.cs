using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory _clientFactory;

        public DatabaseController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var uri = configuration["BackendUri"];
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync(uri);
            return response;
        }
    }
}

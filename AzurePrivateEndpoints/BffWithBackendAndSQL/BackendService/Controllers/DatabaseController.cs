using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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

        public DatabaseController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var addressesString = new StringBuilder();
            var addresses = Dns.GetHostAddresses(configuration["ServerName"]);
            foreach (var hostAddress in addresses)
            {
                addressesString.AppendFormat("{0}\n", hostAddress);
            }

            try
            {
                using var conn = new SqlConnection(configuration["ConnectionString"]);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 'Hi' AS Greet";
                var result = (string)await cmd.ExecuteScalarAsync();
                return addressesString.ToString() + '\n' + result;
            }
            catch (Exception ex)
            {
                return addressesString.ToString() + '\n' + ex.Message;
            }
        }
    }
}

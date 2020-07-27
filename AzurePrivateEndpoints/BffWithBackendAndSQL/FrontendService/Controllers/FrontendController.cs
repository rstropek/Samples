using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontendService.Controllers
{
    [Route("")]
    [ApiController]
    public class FrontendController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory _clientFactory;

        public FrontendController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var resultText = new StringBuilder();
            try
            {
                var addresses = Dns.GetHostAddresses(configuration["ServerName"]);
                resultText.Append($"Resolved name '{configuration["ServerName"]}':\n");
                foreach (var hostAddress in addresses)
                {
                    resultText.AppendFormat("{0}\n", hostAddress);
                }
            }
            catch (Exception ex)
            {
                resultText.Append($"ERROR while resolving name '{configuration["ServerName"]}':\n");
                resultText.Append(ex.Message);
            }

            resultText.Append('\n');

            try
            {
                var uri = configuration["BackendUri"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetStringAsync(uri);
                resultText.Append($"Response from '{configuration["BackendUri"]}':\n");
                resultText.Append(response);
            }
            catch (Exception ex)
            {
                resultText.Append($"ERROR while getting response from '{configuration["BackendUri"]}':\n");
                resultText.Append(ex.Message);
            }

            return resultText.ToString();
        }
    }
}

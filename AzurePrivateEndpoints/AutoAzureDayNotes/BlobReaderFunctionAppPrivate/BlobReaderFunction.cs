using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Storage.Blobs;
using System.Text;
using Azure.Storage.Blobs.Specialized;
using Azure.Core;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace BlobReaderFunctionApp
{
    public static class BlobReaderFunction
    {
        [FunctionName("BlobReader")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Starting to process request");

            var resultBuilder = new StringBuilder();
            try
            {
                resultBuilder.Append("Claims:\n");
                foreach (var claim in req.HttpContext.User.Claims)
                {
                    resultBuilder.Append($"    {claim.Type}: {claim.Value}\n");
                }
                resultBuilder.Append("\n");

                // Manually get access token for storage (demo purposes only)
                // NEVER LOG TOKENS LIKE THAT IN PRODUCTION!
                var cred = new DefaultAzureCredential();
                var token = await cred.GetTokenAsync(new TokenRequestContext(new[] { "https://storage.azure.com/.default" }));
                resultBuilder.Append($"Token: {token.Token}\n\n");

                var addresses = Dns.GetHostAddresses("stvwazuredayprivate.blob.core.windows.net");
                foreach (var hostAddress in addresses)
                {
                    resultBuilder.Append($"Address: {hostAddress}\n\n");
                }

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                client.DefaultRequestHeaders.Add("x-ms-version", "2017-11-09");

                var message = await client.GetStringAsync("https://stvwazuredayprivate.blob.core.windows.net/vwazureday/hello-world.txt");

                log.LogInformation("Done processing the request");
                resultBuilder.Append($"Message: {message}\n\n");
            }
            catch (Exception ex)
            {
                resultBuilder.Append($"Exception: {ex.Message}\n\n");
            }

            return new OkObjectResult(resultBuilder.ToString());
        }
    }
}

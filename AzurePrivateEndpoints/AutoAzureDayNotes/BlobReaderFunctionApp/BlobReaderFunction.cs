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

namespace BlobReaderFunctionApp
{
    public static class BlobReaderFunction
    {
        [FunctionName("BlobReader")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("e request");

            // Manually get access token for storage (demo purposes only)
            // NEVER LOG TOKENS LIKE THAT IN PRODUCTION!
            var cred = new DefaultAzureCredential();
            var token = await cred.GetTokenAsync(new TokenRequestContext(new[] { "https://storage.azure.com/.default" }));
            log.LogInformation(token.Token);

            // Download a blob with the Azure-provided token
            const string containerEndpoint = "https://stmymicroservice.blob.core.windows.net/itdays/";
            var containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());
            var blockBlobClient = containerClient.GetBlockBlobClient("hello-world.txt");
            using var memorystream = new MemoryStream();
            await blockBlobClient.DownloadToAsync(memorystream);
            var message = Encoding.UTF8.GetString(memorystream.ToArray());

            log.LogInformation("Done processing the request");
            return new OkObjectResult(message);
        }
    }
}

using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Function
{
    public class BlobHandling
    {
        private readonly ILogger<BlobHandling> logger;

        public record GetBlobResult(string? BlobContent = null, string? IPAddresses = null, string? ErrorMessage = null);

        public BlobHandling(ILogger<BlobHandling> logger)
        {
            this.logger = logger;
        }

        [Function(nameof(GetBlob))]
        public async Task<HttpResponseData> GetBlob([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var resultDto = new GetBlobResult();

            try
            {
                var containerEndpoint = Environment.GetEnvironmentVariable("StorageConnection")!;

                var hostname = containerEndpoint[8..containerEndpoint.LastIndexOf('/')];
                logger.LogInformation($"Resolving hostname {hostname}");
                var ip = DnsUtil.ResolveDnsName(hostname);
                resultDto = resultDto with { IPAddresses = ip };

                var containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());
                var helloWorldBlob = containerClient.GetBlobClient("hello-world.txt");
                var helloWorldContent = await helloWorldBlob.DownloadContentAsync();
                var helloWorldMessage = Encoding.UTF8.GetString(helloWorldContent.Value.Content);
                resultDto = resultDto with { BlobContent = helloWorldMessage };

            }
            catch (Exception ex)
            {
                resultDto = resultDto with { ErrorMessage = ex.Message };
            }

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(resultDto);
            return response;
        }
    }
}

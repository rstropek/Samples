using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Sas;
using Azure.Storage.Blobs;

namespace Functions.DotNetCore6InProc
{
    public class BlobAccessFunctions
    {
        private readonly IConfiguration configuration;

        public BlobAccessFunctions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [FunctionName(nameof(GetUploadSasUrl))]
        public IActionResult GetUploadSasUrl(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("%StorageContainer%", Connection = "StorageConnection")] BlobContainerClient container,
            ILogger log)
        {
            // Note that we can get a BlobContainerClient as a parameter.
            // This is possible because of in-process .NET Core.

            // Note HTTP-related data types from ASP.NET Core.

            var filename = $"{configuration["Prefix"]}{Guid.NewGuid()}.csv";
            log.LogInformation("Generating SAS for file {file}", filename);

            // Note the use of the lastest blob storage SDK.
            var sasBuilder = new BlobSasBuilder
            {
                BlobName = filename,
                Resource = "b",
                ExpiresOn = DateTime.Now.AddMinutes(5),
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Add | BlobSasPermissions.Create | BlobSasPermissions.Write);
            var blobClient = container.GetBlobClient(filename);
            var sas = blobClient.GenerateSasUri(sasBuilder);

            return new OkObjectResult(new { UploadSas = sas.ToString() });
        }
    }
}

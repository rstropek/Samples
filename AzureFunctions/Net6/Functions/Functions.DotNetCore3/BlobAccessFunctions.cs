using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using System;
using Microsoft.Extensions.Configuration;

namespace Functions.DotNetCore3
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
            [Blob("%StorageContainer%", Connection = "StorageConnection")] CloudBlobContainer container,
            ILogger log)
        {
            // Note that we can get a CloudBlobContainer as a parameter.
            // This is possible because of in-process .NET Core.

            // Note HTTP-related data types from ASP.NET Core.

            var filename = $"{configuration["Prefix"]}{Guid.NewGuid()}.csv";
            log.LogInformation("Generating SAS for file {file}", filename);

            var blob = container.GetBlockBlobReference(filename);
            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Write,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(5)
            });

            return new OkObjectResult(new { UploadSas = blob.Uri + sas });
        }
    }
}

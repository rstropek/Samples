using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;
using Azure.Core.Serialization;
using Azure.Storage.Sas;
using Azure.Storage.Blobs;

namespace Functions.DotNetCore6
{
    public class BlobAccessFunctions
    {
        private readonly ILogger<BlobAccessFunctions> log;
        private readonly IConfiguration configuration;

        public BlobAccessFunctions(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;

            // Note that in .NET Isolated, we need to use the
            // logger factory to get a concrete logger.
            log = loggerFactory.CreateLogger<BlobAccessFunctions>();
        }

        [Function(nameof(GetUploadSasUrl))]
        public async Task<HttpResponseData> GetUploadSasUrl(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            // Note that we cannot use types like CloudBlobContainer because
            // our .NET 6 function runs isolated in a separate process. Everything
            // has to be marshalled accross process boundaries (gRPC).

            // Note HTTP-related data types separated from ASP.NET Core.

            var filename = $"{configuration["Prefix"]}{Guid.NewGuid()}.csv";
            log.LogInformation("Generating SAS for file {file}", filename);

            // Note the use of the lastest blob storage SDK.
            var container = configuration["StorageContainer"];
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container,
                BlobName = filename,
                Resource = "b",
                ExpiresOn = DateTime.Now.AddMinutes(5),
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Add | BlobSasPermissions.Create | BlobSasPermissions.Write);
            var blobClient = new BlobClient(configuration["StorageConnection"], container, filename);
            var sas = blobClient.GenerateSasUri(sasBuilder);

            // Note how we have to use the Function SDK to generate response.
            // We have to do some things manually that we would otherwise get from ASP.NET Core
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(new { UploadSas = sas.ToString() }, new JsonObjectSerializer(new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }));
            return response;
        }
    }
}

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using System.Text.Json;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Text;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace Functions.DotNetCore6InProc
{
    public class BlobProcessFunctions
    {
        public BlobProcessFunctions()
        {
        }

        [FunctionName(nameof(ProcessCsv))]
        public async Task ProcessCsv(
            [BlobTrigger("%StorageContainer%/{name}", Connection = "StorageConnection")] BlobClient csvBlob,
            string name,
            [ServiceBus("%SuccessTopic%", Connection = "ServiceBusConnection", EntityType = ServiceBusEntityType.Topic)] IAsyncCollector<ServiceBusMessage> sender,
            ILogger log)
        {
            // Note the use of IAsyncCollector to send multiple
            // messages to Service Bus.

            log.LogInformation("Parsing file {file}", name);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            // Note that we can ask Functions to give us a BlobClient.
            // With that, we can read data using streams. Could be important
            // for e.g. very large blobs that does not fit into memory.

            using var importStream = new MemoryStream();
            await csvBlob.DownloadToAsync(importStream);
            importStream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(importStream);
            using var csv = new CsvReader(reader, config);

            await foreach (var record in csv.GetRecordsAsync<Something>())
            {
                // Note that we have to manually send message by message.
                await sender.AddAsync(CreateMessage(record, name));
            }
        }

        public ServiceBusMessage CreateMessage(Something item, string fileName)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item));
            var message = new ServiceBusMessage(bytes) { ContentType = "application/json" };
            message.ApplicationProperties["FileName"] = fileName;

            // For demo purposes, we manipulate a setting in the generated Service Bus message.
            // We can influence all details of the message because we can access the
            // Service Bus native type.
            message.ScheduledEnqueueTime = DateTime.UtcNow.AddSeconds(5);
            return message;
        }
    }
}

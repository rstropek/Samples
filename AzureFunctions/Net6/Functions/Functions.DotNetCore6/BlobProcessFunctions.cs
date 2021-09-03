using Microsoft.Extensions.Logging;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;

namespace Functions.DotNetCore6
{
    public class BlobProcessFunctions
    {
        private readonly ILogger<BlobProcessFunctions> log;

        public BlobProcessFunctions(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<BlobProcessFunctions>();
        }

        [Function(nameof(ProcessCsv))]
        [ServiceBusOutput("%SuccessTopic%", Connection = "ServiceBusConnection", EntityType = EntityType.Topic)]
        public async Task<string> ProcessCsv(
            [BlobTrigger("%StorageContainer%/{name}", Connection = "StorageConnection")] string csvBlob,
            string name,
            FunctionContext context)
        {
            // Note that we receive the content of the blob in a string.
            // We cannot ask for e.g. a stream or a BlobClient.

            log.LogInformation("Parsing file {file}", name);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            using var reader = new StringReader(csvBlob);
            using var csv = new CsvReader(reader, config);

            var result = new List<Something>();
            await foreach (var record in csv.GetRecordsAsync<Something>())
            {
                result.Add(record);
            }

            log.LogInformation("Parsing successful, sending message");

            // Note that we return all items in a serialized collection. The isolated
            // runtime will turn the collection into a series of messages.
            // We can only influence the content of the messages. We cannot control
            // other message parameters (e.g. ScheduledEnqueueTimeUtc, TTL).
            return JsonSerializer.Serialize(result);
        }
    }
}

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Functions.DotNetCore6
{
    public class DataProcessingFunction
    {
        private readonly ILogger<DataProcessingFunction> log;

        public DataProcessingFunction(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<DataProcessingFunction>();
        }

        [Function(nameof(DataProcessing))]
        public void DataProcessing(
            [ServiceBusTrigger("%SuccessTopic%", "%SuccessSubscription%", Connection = "ServiceBusConnection")] string message,
            FunctionContext context)
        {
            var data = JsonSerializer.Deserialize<Something>(message);

            log.LogInformation($"Received parsed data, processing it: {data!.Name}");
        }
    }
}

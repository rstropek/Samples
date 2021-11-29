namespace Functions.DotNetCore6InProc;

public class DataProcessingFunction
{
    public DataProcessingFunction()
    {
    }

    [FunctionName(nameof(DataProcessing))]
    public void DataProcessing(
        [ServiceBusTrigger("%SuccessTopic%", "%SuccessSubscription%", Connection = "ServiceBusConnection")] string message,
        ILogger log)
    {
        var data = JsonSerializer.Deserialize<Something>(message);

        log.LogInformation($"Received parsed data, processing it: {data!.Name}");
    }
}

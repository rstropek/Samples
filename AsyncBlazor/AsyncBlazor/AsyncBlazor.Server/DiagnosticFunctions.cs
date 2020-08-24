using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AsyncBlazor.Server
{
    public class DiagnosticFunctions
    {
        [FunctionName("Ping")]
        public string Ping(
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
#pragma warning restore IDE0060 // Remove unused parameter
            ILogger log)
        {
            log.LogInformation("Received ping, answering with pong");
            return "Pong";
        }
    }
}

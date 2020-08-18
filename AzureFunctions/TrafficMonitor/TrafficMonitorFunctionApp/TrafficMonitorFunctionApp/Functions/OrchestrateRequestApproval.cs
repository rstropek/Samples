using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TrafficMonitor.Model;

namespace TrafficMonitorFunctionApp.Functions
{
    public static class OrchestrateRequestApproval
    {
        [FunctionName("OrchestrateRequestApproval")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var approvalRequest = context.GetInput<PlateReadApproval>();
            approvalRequest.InstanceId = context.InstanceId;

            await context.CallActivityAsync("SendApprovalRequestViaSlack", approvalRequest);

            // Wait for Response as an external event or a time out. 
            // The approver has a limit to approve otherwise the request will be rejected.
            using var timeoutCts = new CancellationTokenSource();
            var expiration = context.CurrentUtcDateTime.AddMinutes(5);
            Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

            var approvalResponse = context.WaitForExternalEvent<bool>("ReceiveApprovalResponse");
            var winner = await Task.WhenAny(approvalResponse, timeoutTask);
            if (winner == approvalResponse && approvalResponse.Result)
            {
                log.LogInformation("License plate read approved");
            }
            else
            {
                log.LogInformation("License plate read rejected");
            }

            if (!timeoutTask.IsCompleted)
            {
                // All pending timers must be completed or cancelled before the function exits.
                timeoutCts.Cancel();
            }

            // Once the approval process has been finished, the Blob is to be moved to the corresponding container.
            return winner == approvalResponse;
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TrafficMonitorFunctionApp.Functions
{
    public class ProcessSlackApprovals
    {
        private readonly HttpClient httpClient;

        public ProcessSlackApprovals(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Processes Slack Interactive Message Responses.
        /// Responses are received as 'application/x-www-form-urlencoded'
        /// Routes the response to the corresponding Durable Function orchestration instance 
        /// More information at https://api.slack.com/docs/message-buttons
        /// I'm using AuthorizationLevel.Anonymous just for demostration purposes, but you most probably want to authenticate and authorise the call. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ProcessSlackApprovals")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "slackapproval")] HttpRequestMessage req, [OrchestrationClient] DurableOrchestrationClient orchestrationClient, ILogger log)
        {
            var formData = await req.Content.ReadAsFormDataAsync();
            string payload = formData.Get("payload");
            dynamic response = JsonConvert.DeserializeObject(payload);
            string callbackId = response.callback_id;
            string responseUrl = response.response_url;
            string[] callbackIdParts = callbackId.Split('#');
            string approvalType = callbackIdParts[0];
            log.LogInformation($"Received a Slack Response with callbackid {callbackId}");

            string instanceId = callbackIdParts[1];
            bool isApproved = false;
            log.LogInformation($"instaceId:'{instanceId}', response:'{response.actions[0].value}'");
            var status = await orchestrationClient.GetStatusAsync(instanceId);
            log.LogInformation($"Orchestration status: '{status}'");
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running || status.RuntimeStatus == OrchestrationRuntimeStatus.Pending)
            {
                string selection = response.actions[0].value;
                isApproved = selection == "Approve";

                await orchestrationClient.RaiseEventAsync(instanceId, "ReceiveApprovalResponse", isApproved);

                httpClient.BaseAddress = new Uri(responseUrl);
                var responseMessage = Environment.GetEnvironmentVariable("Slack:ResponseMessage", EnvironmentVariableTarget.Process);
                var content = new StringContent(responseMessage, UnicodeEncoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(responseUrl, content);

                return new OkObjectResult($"Thanks for your selection! Your selection was *'{selection}'*");
            }
            else
            {
                return new OkObjectResult($"The approval request has expired!");
            }
        }
    }
}

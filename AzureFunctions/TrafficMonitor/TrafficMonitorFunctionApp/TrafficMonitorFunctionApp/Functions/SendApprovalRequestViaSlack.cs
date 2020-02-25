using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TrafficMonitor.Model;

namespace TrafficMonitorFunctionApp.Functions
{
    public class SendApprovalRequestViaSlack
    {
        private readonly HttpClient httpClient;

        public SendApprovalRequestViaSlack(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("SendApprovalRequestViaSlack")]
        public async Task<string> Run([ActivityTrigger] PlateReadApproval requestMetadata, ILogger log)
        {
            var approvalRequestUrl = Environment.GetEnvironmentVariable("Slack:ApprovalUrl", EnvironmentVariableTarget.Process);
            var approvalMessageTemplate = Environment.GetEnvironmentVariable("Slack:ApprovalMessageTemplate", EnvironmentVariableTarget.Process);
            var approvalMessage = string.Format(approvalMessageTemplate, requestMetadata.Read.LicensePlate, requestMetadata.InstanceId);

            string resultContent;
            httpClient.BaseAddress = new Uri(approvalRequestUrl);
            var content = new StringContent(approvalMessage, UnicodeEncoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(approvalRequestUrl, content);
            resultContent = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException(resultContent);
            }

            log.LogInformation($"Message regarding {requestMetadata.Read.LicensePlate} sent to Slack!");
            return resultContent;
        }
    }
}

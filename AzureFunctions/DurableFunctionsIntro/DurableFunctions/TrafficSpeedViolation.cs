using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using Microsoft.AspNetCore.Mvc;

namespace DurableFunctions
{
    #region Data Transfer Objects
    /// <summary>
    /// Represents a speed violation recognized by a traffic camera
    /// </summary>
    public class SpeedViolation
    {
        /// <summary>
        /// ID of the camera that has recognized the vehicle
        /// </summary>
        public int CameraID { get; set; }

        /// <summary>
        /// License plate number as recognized by the camera
        /// </summary>
        public string LicensePlateNumber { get; set; }

        /// <summary>
        /// Accuracy of license plate recognition (value between 0 and 1)
        /// </summary>
        public double RecognitionAccuracy { get; set; }

        /// <summary>
        /// Measured speed of the vehicle
        /// </summary>
        public decimal SpeedKmh { get; set; }
    }

    /// <summary>
    /// Represents a request for manual approval of license plate read
    /// </summary>
    public class ApprovalRequest
    {
        /// <summary>
        /// ID or the long-running orchestration handling the approval process
        /// </summary>
        public string OrchestrationInstanceID { get; set; }


        /// <summary>
        /// Data about the speed violation to approve
        /// </summary>
        public SpeedViolation SpeedViolation { get; set; }
    }

    /// <summary>
    /// Represents a response of a user concerning a license plate read
    /// </summary>
    public class ApprovalResponse
    {
        /// <summary>
        /// ID or the long-running orchestration handling the approval process
        /// </summary>
        public string OrchestrationInstanceID { get; set; }

        /// <summary>
        /// True if license plate read has been confirmed, otherwise false
        /// </summary>
        public bool Approved { get; set; }
    }
    #endregion

    public class TrafficSpeedViolation
    {
        /// <summary>
        /// Web API handling incoming speed violations recognized by traffic cameras
        /// </summary>
        /// <returns>
        /// OK if license plate read accuracy was ok, otherwise tracking data for
        /// long-running orchestration handling manual approval of license plate read.
        /// </returns>
        [FunctionName(nameof(SpeedViolationRecognition))]
        public async Task<HttpResponseMessage> SpeedViolationRecognition(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Get speed violation data from HTTP body
            var sv = JsonSerializer.Deserialize<SpeedViolation>(await req.Content.ReadAsStringAsync());

            // Check if read accuracy was not good enough
            if (sv.RecognitionAccuracy < 0.75d)
            {
                log.LogInformation($"Recognition not accurate enough, starting orchestration to ask human for help");
                
                // Start durable function for manual approval process
                string instanceId = await starter.StartNewAsync(nameof(ManuallyApproveRecognition), sv);

                // Return status object with instance ID and URLs for status monitoring
                return starter.CreateCheckStatusResponse(req, instanceId);
            }

            // Read accuracy was ok -> store it (assumption: storing speed 
            // violation is pretty fast, i.e. a matter of seconds).
            await StoreSpeedViolation(sv, log);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName(nameof(GetLawsuite))]
        public async Task<IActionResult> GetLawsuite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lawsuite/{entityKey}")]HttpRequestMessage req,
            string entityKey,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var svl = await client.ReadEntityStateAsync<SpeedViolationLawsuit>(new EntityId(nameof(SpeedViolationLawsuit), entityKey));

            return new OkObjectResult(svl);
        }

        [FunctionName(nameof(SetDriver))]
        public async Task<IActionResult> SetDriver(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lawsuite/{entityKey}/setDriver")]HttpRequestMessage req,
            string entityKey,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var driverName = JsonSerializer.Deserialize<string>(await req.Content.ReadAsStringAsync());
            await client.SignalEntityAsync(new EntityId(nameof(SpeedViolationLawsuit), entityKey), nameof(SpeedViolationLawsuit.StoreDriver), driverName);
            return new StatusCodeResult((int)HttpStatusCode.Accepted);
        }

        private const string ReceiveApprovalResponseEvent = "ReceiveApprovalResponse";

        /// <summary>
        /// Web API receiving responses from Slack API
        /// </summary>
        /// <returns>
        /// OK if approval was ok, BadRequest if approval is unknown or no longer running
        /// </returns>
        [FunctionName(nameof(ProcessSlackApproval))]
        public async Task<HttpResponseMessage> ProcessSlackApproval(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient orchestrationClient,
            ILogger log)
        {
            // Get approval response from HTTP body
            var slackResponse = JsonSerializer.Deserialize<ApprovalResponse>(await req.Content.ReadAsStringAsync());

            // Get status based on orchestration ID
            var status = await orchestrationClient.GetStatusAsync(slackResponse.OrchestrationInstanceID);
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running || status.RuntimeStatus == OrchestrationRuntimeStatus.Pending)
            {
                log.LogInformation("Received Slack response in time, raising event");

                // Raise an event for the given orchestration
                await orchestrationClient.RaiseEventAsync(slackResponse.OrchestrationInstanceID,
                    ReceiveApprovalResponseEvent, slackResponse.Approved);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
             
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Durable function handling long-running approval process
        /// </summary>
        [FunctionName(nameof(ManuallyApproveRecognition))]
        public async Task<bool> ManuallyApproveRecognition([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            // Get speed violation data from orchestration context
            var sv = context.GetInput<SpeedViolation>();

            // Call activity that sends approval request to Slack. Note that this
            // activity will not await the human's response. It will only wait until
            // message will have been sent to Slack.
            await context.CallActivityAsync(nameof(SendApprovalRequestViaSlack), new ApprovalRequest
            {
                OrchestrationInstanceID = context.InstanceId,
                SpeedViolation = sv
            });

            // We want the human operator to respond within 60 minutes. We setup a
            // timer for that. Note that this is NOT a regular .NET timer. It is a
            // special timer from the Durable Functions runtime!
            using var timeoutCts = new CancellationTokenSource();
            var expiration = context.CurrentUtcDateTime.AddMinutes(60);
            var timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

            // Wait for the event that will be raised once we have received the response from Slack.
            var approvalResponse = context.WaitForExternalEvent<bool>(ReceiveApprovalResponseEvent);

            // Wait for Slack response or timer, whichever comes first
            var winner = await Task.WhenAny(approvalResponse, timeoutTask);

            // Was the Slack task the first task to complete?
            if (winner == approvalResponse && approvalResponse.Result)
            {
                // License plate read approved -> Store speed violation
                await context.CallActivityAsync(nameof(StoreSpeedViolation), sv);

                var entityId = context.NewGuid();
                var lawsuitId = new EntityId(nameof(SpeedViolationLawsuit), entityId.ToString());
                await context.CallEntityAsync(lawsuitId, nameof(SpeedViolationLawsuit.SetSpeedViolation), sv);
                log.LogInformation(entityId.ToString());
            }

            if (!timeoutTask.IsCompleted)
            {
                // All pending timers must be completed or cancelled before the function exits.
                timeoutCts.Cancel();
            }

            return winner == approvalResponse && approvalResponse.Result;
        }

        [FunctionName(nameof(SendApprovalRequestViaSlack))]
        public Task SendApprovalRequestViaSlack([ActivityTrigger] ApprovalRequest req, ILogger log)
        {
            log.LogInformation($"Message regarding {req.SpeedViolation.LicensePlateNumber} sent to Slack " +
                $"(instance ID {req.OrchestrationInstanceID}!");

            // Todo: Send data about speed violation to Slack via Slack REST API.
            //       Not implemented here, just a demo.

            return Task.CompletedTask;
        }

        [FunctionName(nameof(StoreSpeedViolation))]
        public Task StoreSpeedViolation([ActivityTrigger] SpeedViolation sv, ILogger log)
        {
            log.LogInformation($"Processing speed violation from camera {sv.CameraID}" +
                $"for LP {sv.LicensePlateNumber} ({sv.SpeedKmh} km/h)");

            // Todo: Add code for processing speed violation
            //       Not implemented here, just a demo.

            return Task.CompletedTask;
        }
    }
}
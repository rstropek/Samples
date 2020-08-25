using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AsyncBlazor.Model;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using AsyncBlazor.OrderProcessing;
using System;

namespace AsyncBlazor.Server
{
    public class OrderProcessingFunctions
    {
        private readonly OrderProcessor processor;
        private readonly Authentication auth;

        public OrderProcessingFunctions(OrderProcessor processor, Authentication auth)
        {
            this.processor = processor;
            this.auth = auth;
        }

        [FunctionName("Orders")]
        public async Task<IActionResult> ReceiveOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [ServiceBus("incoming-orders", Connection = "ServiceBusConnectionString", EntityType = EntityType.Topic)] IAsyncCollector<Message> ordersTopic,
            ILogger log)
        {
            log.LogInformation("Received order, checking access token");

            // Validate token and extract userid from subject claim.
            // Note that this is a naive implementation. In practise, use OpenID Connect
            // ideally in conjunction with Azure Active Directory. This sample is not about auth, so we
            // keep it simple here.
            string? user;
            try
            {
                user = auth.ValidateTokenAndGetUser(req);
            }
            catch (SecurityTokenValidationException ex)
            {
                log.LogError(ex, ex.Message);
                return new UnauthorizedResult();
            }

            // Light-weight order checking
            log.LogInformation("Verifying order");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order;
            try
            {
                order = JsonConvert.DeserializeObject<Order>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                log.LogError(ex, ex.Message);
                return new BadRequestErrorMessageResult(ex.Message);
            }

            if (!await processor.VerifyOrderAsync(order))
            {
                log.LogError("Verification of order failed");
                return new BadRequestResult();
            }

            // Add user id from token to order data
            order.UserID = user;

            // Assign order ID
            order.OrderID = Guid.NewGuid();

            // Send order to SB topic
            await ordersTopic.AddAsync(CreateOrderMessage(order));

            // Respond with Created
            return new CreatedResult(string.Empty, order);
        }

        /// <summary>
        /// Helper function to create SB message from order
        /// </summary>
        private Message CreateOrderMessage(Order order)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order));
            var message = new Message(bytes) { ContentType = "application/json" };
            return message;
        }

        [FunctionName("ProcessOrder")]
        public async Task ProcessOrder(
            [ServiceBusTrigger("incoming-orders", "incoming-orders-process", Connection = "ServiceBusConnectionString")] Order order,
            [SignalR(HubName = nameof(OrderProcessingHub))] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation($"Received order {JsonConvert.SerializeObject(order)} for processing");

            // Send progress reporting via SignalR output binding
            var progress = new Progress<string>();
            progress.ProgressChanged += async (_, ea) =>
                await signalRMessages.AddAsync(BuildMessage(order, ea));

            // Trigger complex order processing
            if (await processor.ProcessOrderAsync(order, progress))
            {
                // Order processing was completed successfull. Send notification 
                // to browser client by using a SignalR output binding.
                await signalRMessages.AddAsync(BuildMessage(order, $"Processing of order {order.OrderID} completed"));
            }
        }

        private SignalRMessage BuildMessage(Order order, string message) =>
            new SignalRMessage
            {
                Target = "OrderEvent",
                UserId = order.UserID,
                Arguments = new object[] { order, message }
            };
    }
}

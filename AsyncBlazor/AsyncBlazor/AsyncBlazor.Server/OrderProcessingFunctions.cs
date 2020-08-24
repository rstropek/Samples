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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.Amqp.Framing;

namespace AsyncBlazor.Server
{
    public class OrderProcessingFunctions
    {
        private readonly Authentication auth;

        public OrderProcessingFunctions(Authentication auth)
        {
            this.auth = auth;
        }

        [FunctionName("Orders")]
        public async Task<IActionResult> ReceiveOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [ServiceBus("incoming-orders", Connection = "ServiceBusConnectionString", EntityType = EntityType.Topic)] IAsyncCollector<Message> ordersTopic,
            ILogger log)
        {
            log.LogInformation("Received order, checking access token");
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

            if (!await VerifyOrderAsync(order))
            {
                log.LogError("Verification of order failed");
                return new BadRequestResult();
            }

            // Add user to order data
            order.UserID = user;

            // Send order to SB topic
            await ordersTopic.AddAsync(CreateOrderMessage(order));

            return new AcceptedResult();
        }
        
        [FunctionName("ProcessOrder")]
        public async Task ProcessOrder(
            [ServiceBusTrigger("incoming-orders", "incoming-orders-process", Connection = "ServiceBusConnectionString")] Order order,
            [SignalR(HubName = nameof(OrderProcessingHub))] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("Received order for processing");

            // Trigger complex order processing
            if (await ProcessOrderAsync(order))
            {
                // Order processing was successfull. Send notification to browser client
                // by using a SignalR output binding.
                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "OrderProcessed",
                    UserId = order.UserID,
                    Arguments = new [] { order }
                });
            }
        }

        internal async Task<bool> VerifyOrderAsync(Order _)
        {
            // Simulate some processing time. Assumption: Initial checking of
            // incoming order is rather fast (e.g. check of master data like 
            // referenced customer exists).
            await Task.Delay(100);

            return true;
        }

        internal async Task<bool> ProcessOrderAsync(Order _)
        {
            // Simulate processing time. Assumption: This is the heavy lifting
            // for order processing. Might include complex calculations, talking
            // to multiple slow backend services, executing complex queries, etc.
            await Task.Delay(2500);

            return true;
        }

        internal Message CreateOrderMessage(Order order)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order));
            var message = new Message(bytes) { ContentType = "application/json" };
            return message;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace AsyncBlazor.Server
{
    public class OrderProcessingHub : ServerlessHub
    {
        private readonly Authentication auth;

        public OrderProcessingHub(Authentication auth)
        {
            this.auth = auth;
        }

        [FunctionName("negotiate")]
        public IActionResult Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            ILogger log)
        {
            // Validate token. Note that this is a naive implementation. In practise, use OpenID Connect
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

            return new OkObjectResult(Negotiate(user));
        }

        // Note that in order to make event handling functions work, you must
        // set the Azure SignalR upstream URL accordingly. For local debugging,
        // use e.g. ngrok to get a URL like https://79569280f5e4.ngrok.io/runtime/webhooks/signalr.
        // For configuration in Azure see https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-signalr-service-trigger?tabs=csharp#send-messages-to-signalr-service-trigger-binding.

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [FunctionName(nameof(OnConnected))]
        public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");
        }

        [FunctionName(nameof(OnDisconnected))]
        public async Task OnDisconnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has disconnected");
        }

        [FunctionName(nameof(SayHelloAsync))]
#pragma warning disable IDE0060 // Remove unused parameter
        public async Task SayHelloAsync([SignalRTrigger] InvocationContext invocationContext, string message, ILogger logger)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            logger.LogInformation($"Got message from client: {message}");
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
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

        [FunctionName(nameof(OnConnected))]
        public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");
        }


        [FunctionName(nameof(SayHelloAsync))]
        public async Task SayHelloAsync([SignalRTrigger] InvocationContext invocationContext, string message, ILogger logger)
        {
            logger.LogInformation(message);
        }
    }
}

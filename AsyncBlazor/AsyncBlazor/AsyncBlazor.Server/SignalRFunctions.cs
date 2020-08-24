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
    public class SignalRFunctions
    {
        private readonly Authentication auth;

        public SignalRFunctions(Authentication auth)
        {
            this.auth = auth;
        }

        //[FunctionName("negotiate")]
        //public IActionResult Negotiate(
        //    [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
        //    [SignalRConnectionInfo(HubName = nameof(OrderProcessingHub), UserId = "{headers.x-ms-signalr-userid}")] SignalRConnectionInfo connectionInfo,
        //    ILogger log)
        //{
        //    // Validate token. Note that this is a naive implementation. In practise, use OpenID Connect
        //    // ideally in conjunction with Azure Active Directory. This sample is not about auth, so we
        //    // keep it simple here.
        //    string? user;
        //    try
        //    {
        //        user = auth.ValidateTokenAndGetUser(req);
        //    }
        //    catch (SecurityTokenValidationException ex)
        //    {
        //        log.LogError(ex, ex.Message);
        //        return new UnauthorizedResult();
        //    }

        //    // Verify that user from token and header are identical.
        //    if (!req.Headers.TryGetValue("x-ms-signalr-userid", out var sruser) || sruser.Count == 0
        //        || sruser[0] != user)
        //    {
        //        return new BadRequestResult();
        //    }

        //    return new OkObjectResult(connectionInfo);
        //}
    }
}

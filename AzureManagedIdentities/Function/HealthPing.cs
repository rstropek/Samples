using System.Text.Json;
using Function;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace UdoApi.Functions;

public class HealthPing
{
    private readonly ITokenSigningService tss;

    public HealthPing(ITokenSigningService tss)
    {
        this.tss = tss;
    }

    [FunctionName(nameof(Ping))]
    public string Ping([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req) => "Pong";

    [FunctionName(nameof(SecurePing))]
    public async Task<IActionResult> SecurePing([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
    {
        var principal = await tss.ValidateAuthorizationHeader(req, log);
        if (principal != null)
        {
            var claims = principal.Claims.Select(c => new { c.Type, c.Value });
            return new OkObjectResult(JsonSerializer.Serialize(claims));
        }

        return new UnauthorizedResult();
    }
}

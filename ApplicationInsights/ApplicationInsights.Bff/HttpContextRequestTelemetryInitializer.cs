using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Security.Claims;

namespace ApplicationInsights.Bff;

/// <summary>
/// Implements a telemetry initializer adding user to request telemetry
/// </summary>
/// <remarks>
/// Read more about telemetry initializers at
/// <see cref="https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#addmodify-properties-itelemetryinitializer"/>.
/// </remarks>
public class HttpContextRequestTelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpContextRequestTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor 
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is not RequestTelemetry requestTelemetry)
        {
            return;
        }

        var claims = httpContextAccessor.HttpContext?.User.Claims;
        if (claims is null)
        {
            return;
        }

        // Get user claim and add it to request telemetry.
        var oidClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
        requestTelemetry.Properties.Add("User", oidClaim?.Value);
    }
}

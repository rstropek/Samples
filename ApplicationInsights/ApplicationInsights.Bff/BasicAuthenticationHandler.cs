using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;

namespace ApplicationInsights.Bff;

/// <summary>
/// Simple authentication handler for Basic authentication
/// </summary>
class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger<BasicAuthenticationHandler> logger;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactor, UrlEncoder encoder, ISystemClock clock,
        ILogger<BasicAuthenticationHandler> logger)
        : base(options, loggerFactor, encoder, clock)
    {
        // Note that we get a logger from ASP.NET Core here. By default, Warning 
        // and more critical errors are logged. You can change that in appsetttings.json.
        // Read more at https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#configure-the-application-insights-sdk
        this.logger = logger;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? userId = null;
        string? user = null;

        // Skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        var header = Request.Headers["Authorization"];
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(header);
            if (authHeader.Scheme != "Basic")
            {
                logger.LogWarning("Invalid authentication schema {Schema}", authHeader.Scheme);
                return Task.FromResult(AuthenticateResult.Fail("Invalid authorization scheme"));
            }

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            // For demo purposes, we have a hardcoded password. NEVER do that in real life.
            // The purpose of this sample is NOT to demonstrate security, but logging.
            // Therefore, we keep everything except logging simple.
            if (password == "P@ssw0rd!")
            {
                userId = "42";
                user = username;
            }
            else
            {
                logger.LogWarning("Invalid password provided for user {User}", username);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invalid Authorization Header: {Header}", header);
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }

        if (user == null || userId == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
        }

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, user),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

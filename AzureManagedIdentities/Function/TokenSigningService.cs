using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AzMgedId.Functions;

public interface ITokenSigningService
{
    Task<ClaimsPrincipal?> ValidateAccessToken(string token, ILogger log);
    Task<ClaimsPrincipal?> ValidateAuthorizationHeader(HttpRequest req, ILogger log);
}

public class TokenSigningService : ITokenSigningService
{
    public async Task<ClaimsPrincipal?> ValidateAccessToken(string token, ILogger log)
    {
        var authorizeUrl = $"https://login.microsoft.com/{Environment.GetEnvironmentVariable("AadTenantId")}/v2.0/.well-known/openid-configuration";

        var openIdConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(authorizeUrl, new OpenIdConnectConfigurationRetriever());
        var openIdConnectConfigData = await openIdConfigurationManager.GetConfigurationAsync();

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            IssuerSigningKeys = openIdConnectConfigData.SigningKeys,
            ValidIssuer = $"https://sts.windows.net/{Environment.GetEnvironmentVariable("AadTenantId")}/",

            ValidAudience = Environment.GetEnvironmentVariable("Audience"),
        };
        var handler = new JwtSecurityTokenHandler();
        try
        {
            #pragma warning disable CA1849
            var principal = handler.ValidateToken(token, validationParameters, out var securityToken);
            if (principal?.Identity?.IsAuthenticated ?? false)
            {
                return principal;
            }
        }
        #pragma warning disable CA1031
        catch (Exception ex)
        {
            log.LogError(ex, "Error while validating token");
        }

        return null;
    }

    public async Task<ClaimsPrincipal?> ValidateAuthorizationHeader(HttpRequest req, ILogger log)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.Headers.TryGetValue("Authorization", out var authHeaders))
        {
            #pragma warning disable CA1310
            if (authHeaders.Count != 1 || !authHeaders[0].StartsWith("Bearer "))
            {
                return null;
            }

            return await ValidateAccessToken(authHeaders[0][(("Bearer ".Length))..], log);
        }

        return null;
    }
}
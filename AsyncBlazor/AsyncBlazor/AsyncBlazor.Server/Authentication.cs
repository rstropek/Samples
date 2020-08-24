using AsyncBlazor.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AsyncBlazor.Server
{
    public class Authentication
    {
        internal string? ValidateTokenAndGetUser(HttpRequest req)
        {
            if (!req.Headers.TryGetValue("Authorization", out var authHeaders) || authHeaders.Count == 0)
            {
                return null;
            }

            TokenValidationParameters validationParameters = new()
            {
                ValidAudience = TokenInfo.Audience,
                ValidIssuer = TokenInfo.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenInfo.Secret))
            };
            JwtSecurityTokenHandler tokendHandler = new();
            tokendHandler.ValidateToken(authHeaders[0][7..], validationParameters, out var jwt);
            if ((jwt as JwtSecurityToken)?.Subject is string username)
            {
                return username;
            }

            throw new SecurityTokenValidationException("Missing Subject claim");
        }
    }
}

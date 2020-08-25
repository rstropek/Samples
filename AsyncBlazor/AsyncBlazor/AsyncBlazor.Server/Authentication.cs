using AsyncBlazor.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AsyncBlazor.Server
{
    /// <summary>
    /// Helper functions for authentication
    /// </summary>
    /// <remarks>
    /// In this sample, real-world authentication is not in scope.
    /// Therefore, a symmetrically signed JWT with dummy secret, 
    /// issuer and audience is used. In practice, use OpenID Connect
    /// ideally with Azure Active Directory for securing your APIs.
    /// </remarks>
    public class Authentication
    {
        /// <summary>
        /// Validate Authorization token and extract user identifier.
        /// </summary>
        /// <param name="req">HTTP request with authorization header</param>
        /// <returns>
        /// User identifier (subject claim) from token if token is valid.
        /// Null if token is invalid or subject claim has not been found.
        /// </returns>
        public string? ValidateTokenAndGetUser(HttpRequest req)
        {
            // Verify that Bearer token is present in authorization header
            if (!req.Headers.TryGetValue("Authorization", out var authHeaders) || authHeaders.Count == 0
                || !authHeaders[0].StartsWith("Bearer "))
            {
                return null;
            }

            // Check if token is valid
            TokenValidationParameters validationParameters = new()
            {
                ValidAudience = TokenInfo.Audience,
                ValidIssuer = TokenInfo.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenInfo.Secret))
            };
            JwtSecurityTokenHandler tokendHandler = new();
            tokendHandler.ValidateToken(authHeaders[0][7..], validationParameters, out var jwt);

            // Extract subject claim
            if ((jwt as JwtSecurityToken)?.Subject is string username)
            {
                return username;
            }

            throw new SecurityTokenValidationException("Missing Subject claim");
        }
    }
}

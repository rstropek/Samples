using AsyncBlazor.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AsyncBlazor.Client
{
    // This is a naive implementation of a helper service for authentication.
    // Note that in practice you should use a ready-made component for this
    // ideally in conjunction with OpenID Connect and Azure Active Directory.
    // Here, auth is kept very simple because proper auth is not in scope
    // for this demo.

    internal class AuthenticationService
    {
        public Task<string> AcquireTokenAsync(string username)
        {
            // Generate a dummy token for the given user.

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("sub", username),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = TokenInfo.Issuer,
                Audience = TokenInfo.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenInfo.Secret)), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Task.FromResult(tokenString);
        }
    }
}

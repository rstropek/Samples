using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NBattleshipCodingContest
{
    public class DummyAuthenticationOptions : AuthenticationSchemeOptions { }

    internal class DummyAuthenticationHandler : AuthenticationHandler<DummyAuthenticationOptions>
    {
        public DummyAuthenticationHandler(IOptionsMonitor<DummyAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Rainer"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false }, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

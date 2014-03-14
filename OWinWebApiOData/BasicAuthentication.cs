using Microsoft.Owin;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
	public class BasicAuthentication : OwinMiddleware
	{
		public BasicAuthentication(OwinMiddleware next)
			: base(next)
		{
		}

		public override async Task Invoke(IOwinContext context)
		{
			var header = context.Request.Headers["Authorization"];

			if (!string.IsNullOrWhiteSpace(header))
			{
				var authHeader = AuthenticationHeaderValue.Parse(header);

				if ("Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
				{
					var unencoded = Convert.FromBase64String(authHeader.Parameter);
					string parameter = Encoding.GetEncoding("iso-8859-1").GetString(unencoded);
					var parts = parameter.Split(':');

					string userName = parts[0];
					string password = parts[1];

					if (userName == "Microsoft" && password == "Day")
					{
						var claims = new[] { new Claim(ClaimTypes.Name, userName) };
						var identity = new ClaimsIdentity(claims, "Basic");
						context.Request.User = new ClaimsPrincipal(identity);
					}
				}
			}

			await Next.Invoke(context);
		}
	}
}
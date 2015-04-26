using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controller
{
	public class SessionController : ApiController
	{
		[Route("api/sessions")]
		[HttpGet]
		public async Task<IHttpActionResult> GetSessions()
		{
			using (var context = new ConferenceContext())
			{
				var sessions = await context.Sessions.ToArrayAsync();
				if (sessions.Length == 0)
				{
					return this.NotFound();
				}

				return this.Ok(sessions);
			}
		}
	}
}
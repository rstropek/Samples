using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace WebApi.Controller
{
	public class TicketController : ApiController
	{
		[Route("api/tickets/{ticketId}")]
		[HttpGet]
		public async Task<IHttpActionResult> GetTicket(string ticketId)
		{
			using (var context = new ConferenceContext())
			{
				var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);
				if (ticket == null)
				{
					return this.NotFound();
				}

				return this.Ok(ticket);
			}
		}

		[Route("api/tickets/{ticketId}", Name = "TicketRoute")]
		[HttpPut]
		public async Task<IHttpActionResult> SaveTicket(Ticket ticket)
		{
			using (var context = new ConferenceContext())
			{
				context.Entry(ticket).State = EntityState.Modified;
				await context.SaveChangesAsync();
				return this.Ok();
			}
		}
	}
}
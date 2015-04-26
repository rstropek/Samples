using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System;

namespace WebApi.Controller
{
	public class RatingController : ApiController
	{
		[Route("api/ratings")]
		[HttpPost]
		public async Task<IHttpActionResult> StoreRatings([FromBody]IEnumerable<SessionRatingDto> ratings)
		{
			using (var context = new ConferenceContext())
			{
				foreach (var rating in ratings.Where(r => r.Rating > 0))
				{
					var existingRating = await context.Ratings.Include("Ticket").Include("Session").FirstOrDefaultAsync(
						rate => rate.Ticket.TicketId == rating.TicketId && rate.Session.SessionId == rating.SessionId);
					if (existingRating == null)
					{
						var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.TicketId == rating.TicketId);
						var session = await context.Sessions.FirstOrDefaultAsync(s => s.SessionId == rating.SessionId);

						var newRating = new SessionRating() { Ticket = ticket, Session = session, Rating = rating.Rating };
						context.Ratings.Add(newRating);
					}
					else
					{
						existingRating.Rating = rating.Rating;
						context.Entry(existingRating).State = EntityState.Modified;
					}

					await context.SaveChangesAsync();
				}
			}

			return this.Ok();
		}
	}
}
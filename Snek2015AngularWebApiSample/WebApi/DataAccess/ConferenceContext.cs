using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApi
{
	public class ConferenceContext : DbContext
	{
		public ConferenceContext()
			: base("ConferenceDB")
		{ }

		public DbSet<Ticket> Tickets { get; set; }

		public DbSet<Session> Sessions { get; set; }

		public DbSet<SessionRating> Ratings { get; set; }
	}
}
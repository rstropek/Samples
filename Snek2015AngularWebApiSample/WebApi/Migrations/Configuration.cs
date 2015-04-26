using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace WebApi.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WebApi.ConferenceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "WebApi.ConferenceContext";
        }

        protected override void Seed(WebApi.ConferenceContext context)
        {
			Ticket ticket;
			context.Tickets.AddOrUpdate(
				t => t.TicketId,
				ticket = new Ticket() { TicketId = "abc123", FirstName = "Max", LastName = "Muster", Email = "max.muster@trash-mail.com" });

			Session s1, s2, s3;
			context.Sessions.AddOrUpdate(
				s => s.SessionId,
				s1 = new Session() { SessionId = new Guid("{BAC9E4AE-2FEF-458F-9C4C-F5C4757F595B}"), Title = "AngularJS Basics" },
				s2 = new Session() { SessionId = new Guid("{A8DCCAFE-26CF-4E57-92CA-DA5E49546F71}"), Title = "Azure Deep Dive" },
				s3 = new Session() { SessionId = new Guid("{293A016E-1FB0-4513-851E-955C66822D59}"), Title = "Fun with Web API" });
        }
    }
}

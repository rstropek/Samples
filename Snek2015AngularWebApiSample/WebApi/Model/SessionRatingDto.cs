using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi
{
	public class SessionRatingDto
	{
		[JsonProperty(PropertyName = "ticketId")]
		public string TicketId { get; set; }

		[JsonProperty(PropertyName = "sessionId")]
		public Guid SessionId { get; set; }

		[JsonProperty(PropertyName = "rating")]
		public int Rating { get; set; }
	}
}
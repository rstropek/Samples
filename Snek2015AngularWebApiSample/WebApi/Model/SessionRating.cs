using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi
{
	public class SessionRating
	{
		public SessionRating()
		{
			this.RatingID = Guid.NewGuid();
		}

		[Key]
		[Required]
		[JsonProperty(PropertyName = "ratingId")]
		public Guid RatingID { get; set; }

		[Required]
		[JsonProperty(PropertyName = "ticket")]
		public Ticket Ticket { get; set; }

		[Required]
		[JsonProperty(PropertyName = "session")]
		public Session Session { get; set; }

		[JsonProperty(PropertyName = "rating")]
		public int Rating { get; set; }
	}
}
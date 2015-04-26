using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApi
{
	public class Ticket
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[JsonProperty(PropertyName = "ticketId")]
		public string TicketId { get; set; }

		[MaxLength(50)]
		[JsonProperty(PropertyName = "firstName")]
		public string FirstName { get; set; }

		[MaxLength(50)]
		[JsonProperty(PropertyName = "lastName")]
		public string LastName { get; set; }

		[MaxLength(50)]
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
	}
}
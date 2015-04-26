using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi
{
	public class Session
	{
		[Key]
		[Required]
		[JsonProperty(PropertyName = "sessionId")]
		public Guid SessionId { get; set; }

		[MaxLength(50)]
		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
	}
}
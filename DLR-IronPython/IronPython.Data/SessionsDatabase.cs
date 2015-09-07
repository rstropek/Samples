using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronPython.Data
{
	public class Session
	{
		[Key]
		public Guid SessionID { get; set; }

		public Guid SpeakerID { get; set; }

		[ForeignKey("SpeakerID")]
		public Speaker Speaker { get; set; }

		public string Title { get; set; }

		public bool Approved { get; set; }
	}

	public class Speaker
	{
		[Key]
		public Guid SpeakerID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Company { get; set; }

		public string Description { get; set; }

		public Collection<Session> Sessions { get; set; }
	}

	public class SamplesEntities : DbContext
	{
		public SamplesEntities()
			: base("Server=(localdb)\\v11.0;Database=IronPython;Integrated Security=true")
		{
		}

		public DbSet<Session> Sessions { get; set; }

		public DbSet<Speaker> Speakers { get; set; }
	}
}

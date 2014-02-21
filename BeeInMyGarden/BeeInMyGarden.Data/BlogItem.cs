using System.ComponentModel.DataAnnotations;

namespace BeeInMyGarden.Data
{
	public class BlogItem
	{
		[Key]
		public int BlogId { get; set; }

		[MaxLength(128)]
		public string Title { get; set; }

		public string Content { get; set; }

		[MaxLength(256)]
		public string FeaturedImageUri { get; set; }
	}
}
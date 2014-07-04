// Note that this example CONTAINS MEMORY LEAKS. I use it in workshops to show how you can use
// memory profilers to find typical .NET memory leaks.

using System.ComponentModel.DataAnnotations;

namespace WpfApplication19
{
	public class Customer
	{
		[Key]
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}

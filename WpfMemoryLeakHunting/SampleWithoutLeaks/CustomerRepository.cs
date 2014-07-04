using System;
using System.Data.Entity;

namespace WpfApplication19
{
	class CustomerRepository : DbContext
	{
		public CustomerRepository() : base(
			"Server=(localdb)\\v11.0;Database=DemoCrm;Integrated Security=true")
		{ }

		public DbSet<Customer> Customers { get; set; }
	}
}

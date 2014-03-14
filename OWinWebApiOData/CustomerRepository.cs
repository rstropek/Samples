using System.Collections.Generic;

namespace WebApi
{
	public static class CustomerRepository
	{
		private static readonly List<Customer> customers = new List<Customer>();

		static CustomerRepository()
		{
			customers.Add(new Customer()
			{
				ID = 1,
				LastName = "Smith",
				FirstName = "Mary",
				HouseNumber = "333",
				Street = "Main Street NE",
				City = "Redmond",
				State = "WA",
				ZipCode = "98053"
			});
		}

		public static IList<Customer> Customers
		{
			get { return customers; }
		}
	}
}
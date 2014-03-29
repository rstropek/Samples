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
			customers.Add(new Customer()
			{
				ID = 1,
				LastName = "John",
				FirstName = "Doe",
				HouseNumber = "111",
				Street = "Broadway NE",
				City = "Seattle",
				State = "WA",
				ZipCode = "98054"
			});
		}

		public static IList<Customer> Customers
		{
			get { return customers; }
		}
	}
}
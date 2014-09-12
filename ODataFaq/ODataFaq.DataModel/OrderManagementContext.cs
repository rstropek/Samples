using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ODataFaq.DataModel
{
	public class OrderManagementContext : DbContext
	{
		public OrderManagementContext()
			: base(@"Server=(localdb)\v11.0;Database=ODataFaq;Integrated Security=true")
		{
		}

		public DbSet<Customer> Customers { get; set; }

		public DbSet<Product> Products { get; set; }

		public DbSet<OrderHeader> Orders { get; set; }

		public DbSet<OrderDetail> OrderDetails { get; set; }

		public async Task ClearAndFillWithDemoData()
		{
			await this.OrderDetails.ForEachAsync(od => this.OrderDetails.Remove(od));
			await this.Orders.ForEachAsync(o => this.Orders.Remove(o));
			await this.Customers.ForEachAsync(c => this.Customers.Remove(c));
			await this.Products.ForEachAsync(p => this.Products.Remove(p));

			var demoCustomers = new[] {
				new Customer() { CompanyName = "Corina Air Conditioning", CountryIsoCode = "AT" },
				new Customer() { CompanyName = "Fernando Engineering", CountryIsoCode = "AT" },
				new Customer() { CompanyName = "Murakami Plumbing", CountryIsoCode = "CH" },
				new Customer() { CompanyName = "Naval Metal Construction", CountryIsoCode = "DE" }
			};
			this.Customers.AddRange(demoCustomers);

			var demoProducts = new[] {
				new Product() { Description = "Mountain Bike", IsAvailable = true, CategoryCode = "BIKE", PricePerUom = 2500 },
				new Product() { Description = "Road Bike", IsAvailable = true, CategoryCode = "BIKE", PricePerUom = 2000 },
				new Product() { Description = "Skate Board", IsAvailable = true, CategoryCode = "BOARD", PricePerUom = 100 },
				new Product() { Description = "Long Board", IsAvailable = true, CategoryCode = "BOARD", PricePerUom = 250 },
				new Product() { Description = "Scooter", IsAvailable = false, CategoryCode = "OTHERS", PricePerUom = 150 }
			};
			this.Products.AddRange(demoProducts);

			var rand = new Random();
			for (var i = 0; i < 100; i++)
			{
				var order = new OrderHeader()
				{
					OrderDate = new DateTimeOffset(new DateTime(2014, rand.Next(1, 12), rand.Next(1, 28))),
					Customer = demoCustomers[rand.Next(demoCustomers.Length - 1)]
				};
				this.Orders.Add(order);

				for (var j = 0; j < 3; j++)
				{
					this.OrderDetails.Add(new OrderDetail()
					{
						Order = order,
						Product = demoProducts[rand.Next(demoProducts.Length - 1)],
						Amount = rand.Next(1, 5)
					});
				}
			}
			
			await this.SaveChangesAsync();
		}
	}
}

using DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApiDemo.Controllers;
using Xunit;

namespace WebApiDemo.Tests
{
    public class CustomerControllerTests
    {
        [Fact]
        public async Task TestAddCustomers()
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementContext>();
            optionsBuilder.UseInMemoryDatabase("DB");

            using (var context = new OrderManagementContext(optionsBuilder.Options))
            {
                var controller = new CustomersController(context);
                await controller.PostCustomer(new Customer
                {
                    CompanyName = "Foo Bar",
                    CountryIsoCode = "AT"
                });

                Assert.NotEmpty(context.Customers);
            }
        }
    }
}

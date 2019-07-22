using DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiDemo.Controllers;
using Xunit;

namespace WebApiDemo.Tests
{
    public class CustomerControllerIntegrationTests
        :  IClassFixture<TestWebApplicationFactory<CustomerControllerIntegrationTests.Startup>>
    {
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDbContext<OrderManagementContext>(options => options.UseInMemoryDatabase("DB"));
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.UseMvc();
            }
        }

        private readonly TestWebApplicationFactory<Startup> factory;

        public CustomerControllerIntegrationTests(TestWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task TestAddCustomersService()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/customers");
            var customers = JsonConvert.DeserializeObject<Customer[]>(await response.Content.ReadAsStringAsync());

            Assert.Empty(customers);

            response = await client.PostAsync(
                new Uri("/api/customers", UriKind.Relative),
                new StringContent(JsonConvert.SerializeObject(new Customer
                {
                    CompanyName = "Foo Bar",
                    CountryIsoCode = "AT"
                }), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync("/api/customers");
            customers = JsonConvert.DeserializeObject<Customer[]>(await response.Content.ReadAsStringAsync());

            Assert.NotEmpty(customers);
        }
    }
}

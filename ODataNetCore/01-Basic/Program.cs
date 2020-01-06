using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;

namespace ODataNetCoreSimple
{
    public class Program
    {
        public static void Main(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                .Run();
    }

    public class Customer
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers, but disable endpoint routing. This is
            // NOT supported yet. The OData team is working on it.
            services.AddControllers(options => options.EnableEndpointRouting = false);

            // Add OData to ASP.NET Core's dependency injection system
            services.AddOData();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routeBuilder =>
            {
                // Specify allowed OData operations
                routeBuilder.Select().Filter().MaxTop(50);

                // Set route name, prefix, and OData data model
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel()
        {
            // Use a convention builder to derive model from C# classes
            // See https://docs.microsoft.com/en-us/odata/webapi/convention-model-builder for details
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            return builder.GetEdmModel();
        }
    }

    public class CustomersController : ODataController
    {
        private static Customer[] Customers { get; } = new [] 
        {
            new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe" },
            new Customer { CustomerId = 2, FirstName = "Foo", LastName = "Bar" }
        };

        [EnableQuery]
        public IActionResult Get() => Ok(Customers.AsQueryable());
    }
}

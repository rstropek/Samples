using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace ODataNetCoreCustomProvider
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
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
            // Add OData to ASP.NET Core's dependency injection system
            services.AddOData();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            // See http://odata.github.io/WebApi/#02-04-convention-model-builder for details
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            return builder.GetEdmModel();
        }
    }

    public class CustomersController : ODataController
    {
        // Helpers for generating customer names
        private readonly char[] letters1 = "aeiou".ToArray();
        private readonly char[] letters2 = "bcdfgklmnpqrstvw".ToArray();
        private Random random = new Random();

        private const int pageSize = 100;

        [EnableQuery]
        [ODataRoute]
        public IActionResult Get(ODataQueryOptions<Customer> options)
        {
            // Calculate number of results based on $top
            var numberOfResults = pageSize;
            if (options.Top != null)
            {
                numberOfResults = options.Top.Value;
            }

            // Analyze $filter
            string equalFilter = null;
            if (options.Filter != null)
            {
                // We only support a single "eq" filter
                var binaryOperator = options.Filter.FilterClause.Expression as BinaryOperatorNode;
                if (binaryOperator == null || binaryOperator.OperatorKind != BinaryOperatorKind.Equal)
                {
                    return BadRequest();
                }

                // One side has to be a reference to CustomerName property, the other side has to be a constant
                var propertyAccess = binaryOperator.Left as SingleValuePropertyAccessNode ?? binaryOperator.Right as SingleValuePropertyAccessNode;
                var constant = binaryOperator.Left as ConstantNode ?? binaryOperator.Right as ConstantNode;
                if (propertyAccess == null || propertyAccess.Property.Name != "LastName" || constant == null)
                {
                    return BadRequest();
                }

                // Save equal filter value
                equalFilter = constant.Value.ToString();

                // Return between 1 and 2 rows (CustomerName is not a primary key)
                numberOfResults = Math.Min(random.Next(1, 3), numberOfResults);
            }

            // Generate result
            var result = new List<Customer>();
            for (var i = 0; i < numberOfResults; i++)
            {
                result.Add(new Customer() { CustomerId = i + 1, FirstName = "Foo", LastName = equalFilter ?? GenerateCustomerName() });
            }

            return Ok(result.AsQueryable());
        }

        private string GenerateCustomerName()
        {
            var length = random.Next(5, 8);
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                var letter = (i % 2 == 0 ? letters1[random.Next(letters1.Length)] : letters2[random.Next(letters2.Length)]).ToString();
                result.Append(i == 0 ? letter.ToUpper() : letter);
            }

            return result.ToString();
        }
    }
}

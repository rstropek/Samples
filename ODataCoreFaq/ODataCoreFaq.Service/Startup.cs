using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using ODataCoreFaq.Data;

namespace ODataCoreFaq.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers, but disable endpoint routing. This is
            // NOT supported yet. The OData team is working on it.
            services.AddControllers(options => options.EnableEndpointRouting = false);

            //services.AddDbContext<OrderManagementContext>(opt => opt.UseInMemoryDatabase("OrderManagement"));
            services.AddDbContext<OrderManagementContext>(options => options.UseSqlServer(
                Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddOData();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseMvc(b =>
            {
                b.Select().Expand().Filter().OrderBy().Count();
                b.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var customers = builder.EntitySet<Customer>("Customers");
            customers
                .EntityType
                .Collection
                .Function("OrderedBike")
                .ReturnsCollectionFromEntitySet<Customer>("Customer");

            builder.EntitySet<Product>("Products");
            builder.EntitySet<OrderHeader>("OrderHeaders");
            builder.EntitySet<OrderDetail>("OrderDetails");

            return builder.GetEdmModel();
        }
    }
}

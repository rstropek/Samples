using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
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
            services.AddDbContext<OrderManagementContext>(options => options.UseSqlServer(
                Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddControllers();
            services.AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5)
                .AddModel("odata", GetEdmModel()));
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
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            builder.EntityType<Customer>()
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

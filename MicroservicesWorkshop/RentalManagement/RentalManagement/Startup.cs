using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RentalManagement.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace RentalManagement
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
            services.Configure<KeyVaultOptions>(Configuration.GetSection("KeyVault"));
            services.Configure<DbServerOptions>(Configuration.GetSection("DB"));

            services.AddSingleton<IKeyVaultReader, KeyVaultReader>();
            services.AddScoped<IDataAccess, DataAccess>();
            services.AddScoped<IServiceBusSender, ServiceBusSender>();
            services.AddSingleton<IRating, Rating>();

            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.Formatting = Formatting.Indented; });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Bike Rental API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bike Rental API V1"); });
        }
    }
}

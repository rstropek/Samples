using AspNetCore1Angular2Intro.Controllers;
using AspNetCore1Angular2Intro.Services;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.SwaggerGen;

namespace AspNetCore1Angular2Intro
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure swagger. For more details see e.g.
            // http://damienbod.com/2015/12/13/asp-net-5-mvc-6-api-documentation-using-swagger/
            services.AddSwaggerGen();
            services.ConfigureSwaggerDocument(options => options.SingleApiVersion(
                new Info { Version = "v1", Title = "Demo" }));

            // Use the new configuration model here. Note the use of 
            // different configuration sources (json, env. variables,
            // middleware-specific configuration).
            services.AddOptions();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddApplicationInsightsSettings(developerMode: true);
            var configuration = builder.Build();

            // Publish options read from configuration file. With that,
            // controllers can use ASP.NET DI to get options (see 
            // BooksController).
            services.Configure<BooksDemoDataOptions>(
                configuration.GetSection("booksDemoDataOptions"));

            // Add AI configuration
            services.AddApplicationInsightsTelemetry(configuration);

            // Publish singleton for name generator
            services.AddSingleton(typeof(INameGenerator), typeof(NameGenerator));

            // Configure middlewares (CORS and MVC)
            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Use IIS platform handle to run ASP.NET in IIS
            app.UseIISPlatformHandler();

            // Use file server to serve static files
            app.UseDefaultFiles();
            app.UseFileServer();

            // Use AI request telemetry
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            // Add middlewares (CORS and MVC)
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc();

            // Configure swagger. For more details see e.g.
            // http://damienbod.com/2015/12/13/asp-net-5-mvc-6-api-documentation-using-swagger/
            app.UseSwaggerGen();
            app.UseSwaggerUi();
        }

        // Entry point for the application.
        public static void Main(string[] args) => 
            WebApplication.Run<Startup>(args);
    }
}

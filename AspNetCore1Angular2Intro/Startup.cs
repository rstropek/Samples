using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.SwaggerGen;
using AspNetCore1Angular2Intro.Services;
using Microsoft.Extensions.Configuration;
using AspNetCore1Angular2Intro.Controllers;

namespace AspNetCore1Angular2Intro
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure swagger. For more details see
            // http://damienbod.com/2015/12/13/asp-net-5-mvc-6-api-documentation-using-swagger/
            services.AddSwaggerGen();
            services.ConfigureSwaggerDocument(options => options.SingleApiVersion(
                new Info { Version = "v1", Title = "Demo" }));

            services.AddOptions();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            services.Configure<BooksDemoDataOptions>(builder.Build().GetSection("booksDemoDataOptions"));

            services.AddSingleton(typeof(INameGenerator), typeof(NameGenerator));

            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();

            app.UseDefaultFiles();
            app.UseFileServer();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc();

            // Configure swagger. For more details see
            // http://damienbod.com/2015/12/13/asp-net-5-mvc-6-api-documentation-using-swagger/
            app.UseSwaggerGen();
            app.UseSwaggerUi();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

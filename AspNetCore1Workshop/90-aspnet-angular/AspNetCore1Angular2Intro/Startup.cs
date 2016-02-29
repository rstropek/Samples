using AspNetCore1Angular2Intro.Controllers;
using AspNetCore1Angular2Intro.Services;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.SwaggerGen;
using System;
using System.IO;

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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, IApplicationEnvironment appEnv)
        {
            if (env.IsDevelopment())
            {
                // Running in development mode -> add some dev tools
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage();
            }

            // Just for demo purposes throw an exception if
            // query string contains "exception"
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("exception"))
                {
                    throw new InvalidOperationException("Something bad happened ...");
                }

                await next();
            });

            // Add logging
            loggerFactory.MinimumLevel = LogLevel.Error;
            loggerFactory.AddConsole(true);

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

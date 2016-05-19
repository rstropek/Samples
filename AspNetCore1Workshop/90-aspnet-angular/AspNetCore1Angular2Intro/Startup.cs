using AspNetCore1Angular2Intro.Controllers;
using AspNetCore1Angular2Intro.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AspNetCore1Angular2Intro
{
    public class Startup
    {
        private string contentRootPath;

        public Startup(IHostingEnvironment env)
        {
            this.contentRootPath = env.ContentRootPath;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Note that this sample does currently not contain Swashbuckle as it
            // has not been updated to RC2 (see https://github.com/domaindrivendev/Swashbuckle/issues/768
            // for status).

            // Use the new configuration model here. Note the use of 
            // different configuration sources (json, env. variables,
            // middleware-specific configuration).
            services.AddOptions();
            var builder = new ConfigurationBuilder()
                .SetBasePath(this.contentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddApplicationInsightsSettings(developerMode: true);
            var configuration = builder.Build();

            // Publish options read from configuration file. With that,
            // controllers can use ASP.NET DI to get options (see 
            // BooksController).
            services.Configure<BooksDemoDataOptions>(o => {
                o.MinimumNumberOfBooks = Int32.Parse(configuration.GetSection("booksDemoDataOptions:minimumNumberOfBooks").Value);
                o.MaximumNumberOfBooks = Int32.Parse(configuration.GetSection("booksDemoDataOptions:maximumNumberOfBooks").Value);
            });

            // Add AI configuration
            services.AddApplicationInsightsTelemetry(configuration);

            // Publish singleton for name generator
            services.AddSingleton(typeof(INameGenerator), typeof(NameGenerator));

            // Configure middlewares (CORS and MVC)
            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
            loggerFactory.AddConsole(LogLevel.Error, true);

            // Use file server to serve static files
            app.UseDefaultFiles();
            app.UseFileServer();

            // Use AI request telemetry
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            // Add middlewares (CORS and MVC)
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
              .UseKestrel()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseIISIntegration()
              .UseStartup<Startup>()
              .Build();

            host.Run();
        }
    }
}

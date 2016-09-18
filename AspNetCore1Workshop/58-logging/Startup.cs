using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication
{
    public class Program
    {
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

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // Note that we are using dev mode here. This delivers AppInsights events
                // faster during debugging.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure AppInsights based on appsettings.json file
            services.AddApplicationInsightsTelemetry(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Add console logger. For other loggers see
            // https://docs.asp.net/en/latest/fundamentals/logging.html
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Here we are creating the logger manually.
            // Use ILogger<T> in controllers instead. For details see
            // https://docs.asp.net/en/latest/fundamentals/logging.html
            var startupLogger = loggerFactory.CreateLogger("Startup");
            startupLogger.LogInformation("Building pipeline");
            if (env.IsDevelopment())
            {
                // Try running this sample with
                // set ASPNETCORE_ENVIRONMENT=development
                startupLogger.LogInformation("Using dev settings");
            }

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            app.Map("/works", ab => ab.Run(ctx => ctx.Response.WriteAsync("This works")));
            app.Map("/worksAlso", ab => ab.Run(ctx => ctx.Response.WriteAsync("This works, too")));

            // Lets throw an exception to generate error events
            app.Map("/crashes", ab => ab.Run(ctx => { throw new DivideByZeroException(); }));

            app.Map("/logs", ab => ab.Run(async ctx => {
                // Lets write some custom AppInsights events
                var client = new TelemetryClient();
                client.TrackEvent("I am tracking a custom event");
                
                // Lets log the use of an external dependency (.NET does that for
                // come dependencies like SQL, web requests, etc. automatically)
                await Task.Delay(1000);
                client.TrackDependency("ExternalService", "GET-SOME-DATA", DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1), true);

                await ctx.Response.WriteAsync("This writes some logs");
            }));
        }
    }
}

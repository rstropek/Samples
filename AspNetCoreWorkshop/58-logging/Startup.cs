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
using Microsoft.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;

// In practice, consider a logging library like Serilog
// (see https://serilog.net/).

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Note that we add a simple console logger here.
                    // Details: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            Configuration = config;
            InDevelopmentMode = env.IsDevelopment();
        }

        public IConfiguration Configuration { get; }
        public bool InDevelopmentMode { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var options = new ApplicationInsightsServiceOptions
            {
                InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"]
            };

            if (InDevelopmentMode)
            {
                options.DeveloperMode = true;
            }

            // Configure AppInsights based on appsettings.json file
            services.AddApplicationInsightsTelemetry(options);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Here we are creating the logger manually.
            // Use ILogger<T> in controllers instead. For details see
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging
            var startupLogger = loggerFactory.CreateLogger("Startup");
            startupLogger.LogInformation("Building pipeline");
            if (env.IsDevelopment())
            {
                // Try running this sample with
                // set ASPNETCORE_ENVIRONMENT=development
                startupLogger.LogInformation("Using dev settings");
            }

            app.Map("/works", ab => ab.Run(ctx => ctx.Response.WriteAsync("This works")));
            app.Map("/worksAlso", ab => ab.Run(ctx => ctx.Response.WriteAsync("This works, too")));

            // Lets throw an exception to generate error events
            app.Map("/crashes", ab => ab.Run(ctx => { throw new DivideByZeroException(); }));

            app.Map("/logs", ab => ab.Run(async ctx =>
            {
                // Lets write some custom AppInsights events
                var client = new TelemetryClient();
                client.TrackEvent("I am tracking a custom event");

                // Lets log the use of an external dependency (.NET does that for
                // come dependencies like SQL, web requests, etc. automatically)
                await Task.Delay(1000);
                var dep = new DependencyTelemetry
                {
                    Name = "ExternalService",
                    Data = "GET-SOME-DATA",
                    Timestamp = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromSeconds(1),
                    Success = true
                };
                client.TrackDependency(dep);

                await ctx.Response.WriteAsync("This writes some logs");
            }));
        }
    }
}

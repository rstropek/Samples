using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GenericHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    // Configure host. For details see
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2#host-configuration-1

                    // Try changing environment...
                    // - in hostsettings.json
                    // - via environment variable (SET CONFIG_environment=Something)
                    // - via command line (dotnet run dotnet run --environment=Test)
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "CONFIG_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    // Configure app settings. For details see
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2#configureappconfiguration
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables(prefix: "CONFIG_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    // Configure logging to write logs to the console. For details see
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2#configurelogging
                    configLogging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Adding our hosted service that monitors a folder. For details see
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2#configureservices
                    services.AddHostedService<FileHashingService>();
                })
                // Make sure to gracefully shutdown in case of Ctrl+C.
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}

using Azure.Core.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Json;

namespace Functions.DotNetCore6
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var appSettingsPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
                    var appSettingsPathDev = Path.Combine(Environment.CurrentDirectory, $"appsettings.{ context.HostingEnvironment.EnvironmentName}.json");
                    builder.AddJsonFile(appSettingsPath, optional: true, reloadOnChange: false)
                        .AddJsonFile(appSettingsPathDev, optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                })
                .Build();

            host.Run();
        }
    }
}
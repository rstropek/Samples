using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TrafficMonitor.Services;

[assembly: FunctionsStartup(typeof(TrafficMonitorFunctionApp.Startup))]

namespace TrafficMonitorFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<LicensePlateGenerator>();
            builder.Services.AddSingleton<VehicleCategoryGenerator>();
            builder.Services.AddSingleton<IStorage, Storage>();
            builder.Services.AddSingleton<Configuration>();

            if (Environment.GetEnvironmentVariable("LicensePlateRecognizer") == "OpenALPR")
            {
                builder.Services.AddSingleton<ILicensePlateRecognizer, OpenAlprRecognizer>();
            }
            else
            {
                builder.Services.AddSingleton<ILicensePlateRecognizer, MockupRecognizer>();
            }

        }
    }
}

using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace TrafficMonitor.Services
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        private IServiceProvider _serviceProvider;

        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ServiceCollection();
            RegisterServices(services);
            _serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .BindToInput<dynamic>(i => _serviceProvider.GetRequiredService(i.Type));
        }
        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<LicensePlateGenerator>();
            services.AddSingleton<VehicleCategoryGenerator>();
            services.AddSingleton<IStorage, Storage>();
            services.AddSingleton<Configuration>();

            if (Environment.GetEnvironmentVariable("LicensePlateRecognizer") == "OpenALPR")
            {
                services.AddSingleton<ILicensePlateRecognizer, OpenAlprRecognizer>();
            }
            else
            {
                services.AddSingleton<ILicensePlateRecognizer, MockupRecognizer>();
            }
        }
    }
}

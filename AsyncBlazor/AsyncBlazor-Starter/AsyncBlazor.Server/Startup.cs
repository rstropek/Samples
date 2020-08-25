using AsyncBlazor.OrderProcessing;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AsyncBlazor.Server.Startup))]

namespace AsyncBlazor.Server
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<OrderProcessor>();
        }
    }
}

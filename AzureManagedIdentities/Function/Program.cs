using Function;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(UdoApi.Functions.Startup))]

namespace UdoApi.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<ITokenSigningService, TokenSigningService>();
    }
}

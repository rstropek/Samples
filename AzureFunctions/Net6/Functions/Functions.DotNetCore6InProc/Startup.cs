[assembly: FunctionsStartup(typeof(Functions.DotNetCore3.Startup))]

namespace Functions.DotNetCore3;

// Use dependency injection in .NET Azure Functions.
// Read more at https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var context = builder.GetContext();

        var appSettingsPath = Path.Combine(context.ApplicationRootPath, "appsettings.json");
        var appSettingsPathDev = Path.Combine(context.ApplicationRootPath, $"appsettings.{ context.EnvironmentName}.json");
        builder.ConfigurationBuilder
            .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: false)
            .AddJsonFile(appSettingsPathDev, optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Register application services using dependency injection (similar to ASP.NET Core).
        // builder.Services.AddSingleton...
    }
}

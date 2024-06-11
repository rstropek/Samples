using Azure;
using Azure.AI.OpenAI;
using Microsoft.Data.SqlClient;

namespace DotnetKiCamp;

public static class SqlConnectionExtensions
{
    public static IServiceCollection AddSqlConnection(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<ConnectionStrings>(configuration);

        services.AddTransient(sp =>
        {
            var options = new ConnectionStrings();
            configuration.Bind(options);

            return new SqlConnection(options.AdventureWorks);
        });

        return services;
    }
}

public class ConnectionStrings
{
    public string AdventureWorks { get; set; } = "";
}

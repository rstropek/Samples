using Azure;
using Azure.AI.OpenAI;

namespace DotnetKiCamp;

public static class OpenAIClientExtensions
{
    public static IServiceCollection AddOpenAIClient(this IServiceCollection services, IConfigurationSection configuration)
    {
        // Add OpenAI client settings to the service collection (IOptions pattern)
        services.Configure<OpenAISettings>(configuration);

        var options = new OpenAISettings();
        configuration.Bind(options);

        OpenAIClient client;
        client = new OpenAIClient(options.ApiKey);

        // The OpenAI client is guaranteed to be thread-safe, 
        // so we can register it as a singleton. See also
        // https://devblogs.microsoft.com/azure-sdk/lifetime-management-and-thread-safety-guarantees-of-azure-sdk-net-clients/
        services.AddSingleton(client);

        return services;
    }
}

/// <summary>
/// Represents the settings for the OpenAI client.
/// </summary>
public class OpenAISettings
{
    /// <summary>
    /// The API key for the OpenAI client.
    /// </summary>
    /// <remarks>
    /// For OpenAI, you get your API key from https://platform.openai.com/api-keys.
    /// In Azure OpenAI, you get your resource API key from the Azure portal.
    /// NOTE: If you are using Azure OpenAI, prefer MANAGED IDENTITY over API key
    /// in production scenarios.
    /// </remarks>
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// The endpoint for the OpenAI client.
    /// </summary>
    /// <remarks>
    /// For OpenAI, this setting can be left empty.
    /// In Azure OpenAI, you get your endpoint from the Azure portal.
    /// </remarks>
    public string? Uri { get; set; }

    /// <summary>
    /// The deployment/model name for the OpenAI client.
    /// </summary>
    /// <remarks>
    /// For OpenAI, this is the model name (see https://platform.openai.com/docs/models).
    /// For Azure, this is the name of the deployment that you want to use.
    public string? ModelName { get; set; }

    /// <summary>
    /// The system prompt used in this example program.
    /// </summary>
    public string SystemPrompt { get; set; } = "";
}

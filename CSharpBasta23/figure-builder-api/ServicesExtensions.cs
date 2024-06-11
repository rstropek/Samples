static class ServicesExtensions
{
    /// <summary>
    /// Add Swagger XML documentation.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The services collection.</returns>
    /// <remarks>
    /// This method will check if the XML documentation file exists in the same folder as the
    /// executing assembly, or in the parent folder. If the file is found, it will be included
    /// in the Swagger UI.
    /// </remarks>
    public static IServiceCollection AddSwaggerXmlDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Include XML documentation in Swagger UI

            // Get folder and file name for C# XML documentation file
            var location = Assembly.GetExecutingAssembly().Location;
            var folder = Path.GetDirectoryName(location)!;
            var fileName = Path.GetFileNameWithoutExtension(location);
            var filePath = Path.Combine(folder, $"{fileName}.xml");

            // Check if XML documentation file exists
            var includeXml = File.Exists(filePath);
            if (!includeXml)
            {
                // Check if XML documentation file exists in parent folder
                filePath = Path.Combine(folder, "..", $"{fileName}.xml");
                includeXml = File.Exists(filePath);
            }

            // Add XML documentation file to Swagger UI
            if (includeXml) { options.IncludeXmlComments(filePath); }
        });
        return services;
    }

    /// <summary>
    /// Add CORS policy that allows all origins, methods, and headers.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The services collection.</returns>
    public static IServiceCollection AddCorsAllowAll(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
        });
        return services;
    }

    /// <summary>
    /// Configures JSON options to serialize enums as strings.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The services collection.</returns>
    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
    {
        // Configure JSON options to serialize enums as strings
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Just to be sure, also configure MVC JSON options
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}

using DataAccess;
using FluentValidation;

namespace Registration.Api;

public static class DependencyInjectionExtensions
{
    public static IHostApplicationBuilder AddJsonFileRepository(this IHostApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("RepositorySettings");
        var repositorySettings = section.Get<RepositorySettings>()
            ?? throw new InvalidOperationException("RepositorySettings are not configured");
        var dataPath = repositorySettings.DataFolder
            ?? throw new InvalidOperationException("RepositorySettings.DataFolder is not configured");
        if (!Directory.Exists(dataPath)) { Directory.CreateDirectory(dataPath); }
        builder.Services.AddSingleton<IJsonFileRepository>(_ => new JsonFileRepository(repositorySettings));
        return builder;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(ValidationResultBehavior<,>));
        services.AddMediatR(cfg =>
        {
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationResultBehavior<,>));
            cfg.RegisterServicesFromAssemblyContaining<CreateCampaign>();
        });
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions.Add("trace-id", ctx.HttpContext.TraceIdentifier);
            };
        });
        services.AddSingleton<ResultConverter>();

        return services;
    }

    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();
        return services;
    }
}


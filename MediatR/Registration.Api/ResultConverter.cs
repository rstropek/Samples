using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Registration.Api;

public class ResultConverter(IProblemDetailsService pds, IConfiguration config, ILogger<ResultConverter> logger)
{
    public IResult ToResult<T>(Result<T> response)
    {
        return new ResultConverter<T>(response, pds, config.GetValue<string>("ProblemDetailsUriPrefix") ?? "https://example.com/errors", logger);
    }
}

public class ResultConverter<T>(Result<T> result, IProblemDetailsService pds, string problemDetailsUriPrefix, ILogger logger) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        if (result.IsFailed)
        {
            if (result.Errors.Count > 1)
            {
                logger.LogWarning("Multiple errors occurred: {Errors}. Only the first will be returned.", result.Errors);
            }

            var error = result.Errors.First();
            switch (error)
            {
                case ValidationErrors ve:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ValidationProblemDetails()
                        {
                            Title = "One or more validation errors occurred",
                            Type = $"{problemDetailsUriPrefix}/validation-error",
                            Instance = context.Request.Path,
                            Errors = ve.Errors.Select(e => new KeyValuePair<string, string[]>(e.PropertyName, [e.ErrorMessage])).ToDictionary(),
                        }
                    });
                    break;
                case BadRequest br:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails()
                        {
                            Title = "Bad Request",
                            Type = $"{problemDetailsUriPrefix}/bad-request",
                            Detail = br.Message,
                            Status = StatusCodes.Status400BadRequest,
                        }
                    });
                    break;
                case Concurrency c:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails()
                        {
                            Title = "Concurrency Error",
                            Type = $"{problemDetailsUriPrefix}/concurrency",
                            Detail = c.Message,
                            Status = StatusCodes.Status409Conflict,
                        }
                    });
                    break;
                case Forbidden f:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails()
                        {
                            Title = "Forbidden",
                            Type = $"{problemDetailsUriPrefix}/forbidden",
                            Detail = f.Message,
                            Status = StatusCodes.Status403Forbidden,
                        }
                    });
                    break;
                case NotFound n:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails()
                        {
                            Title = "Not Found",
                            Type = $"{problemDetailsUriPrefix}/not-found",
                            Detail = n.Message,
                            Status = StatusCodes.Status404NotFound,
                        }
                    });
                    break;
                default:
                    await pds.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails
                        {
                            Type = $"{problemDetailsUriPrefix}/internal-server-error",
                            Title = "An error occurred processing your request",
                            Status = StatusCodes.Status500InternalServerError,
                        }
                    });
                    break;
            }
        }
        else
        {
            await context.Response.WriteAsJsonAsync(result.Value);
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Registration.Api;

public class ExceptionToProblemDetailsHandler(IProblemDetailsService pds, IConfiguration config) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetailsUriPrefix = config?.GetValue<string>("ProblemDetailsUriPrefix") ?? "https://example.com/errors";

        return exception switch
        {
            FluentValidation.ValidationException vex => await pds.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ValidationProblemDetails()
                {
                    Title = "One or more validation errors occurred",
                    Type = $"{problemDetailsUriPrefix}/validation-error",
                    Instance = context.Request.Path,
                    Errors = vex.Errors.Select(e => new KeyValuePair<string, string[]>(e.PropertyName, [e.ErrorMessage])).ToDictionary(),
                }
            }),
            BadHttpRequestException bre => await pds.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails()
                {
                    Title = "Bad Request",
                    Type = $"{problemDetailsUriPrefix}/bad-request",
                    Detail = bre.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = {
                        ["innerMessage"] = bre.InnerException?.Message,
                    },
                }
            }),
            _ => await pds.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Type = $"{problemDetailsUriPrefix}/internal-server-error",
                    Title = "An error occurred processing your request",
                    Status = StatusCodes.Status500InternalServerError,
                }
            }),
        };
    }
}
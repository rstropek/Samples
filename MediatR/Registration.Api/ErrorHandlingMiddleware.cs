using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Registration.Api;

public class ExceptionToProblemDetailsHandler(IProblemDetailsService pds, IConfiguration config) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetailsUriPrefix = config?.GetValue<string>("ProblemDetailsUriPrefix") ?? "https://example.com/errors";

        if (exception is FluentValidation.ValidationException vex)
        {
            return await pds.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ValidationProblemDetails()
                {
                    Title = "One or more validation errors occurred",
                    Type = $"{problemDetailsUriPrefix}/validation-error",
                    Instance = context.Request.Path,
                    Errors = vex.Errors.Select(e => new KeyValuePair<string, string[]>(e.PropertyName, [e.ErrorMessage])).ToDictionary(),
                }
            });
        }

        return await pds.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Type = $"{problemDetailsUriPrefix}/internal-server-error",
                Title = "An error occurred processing your request",
                Status = StatusCodes.Status500InternalServerError,
            }
        });
    }
}
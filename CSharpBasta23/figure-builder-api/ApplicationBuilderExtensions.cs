static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder HandleException<T>(this IApplicationBuilder app, HttpStatusCode statusCode) where T : Exception
    {
        // Add a global exception handler
        return app.UseExceptionHandler(exceptionHandlerApp =>
            exceptionHandlerApp.Run(async context =>
            {
                // Retrieve the exception using the IExceptionHandlerPathFeature
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error is T ex)
                {
                    // The exception is one that we want to handle
                    context.Response.StatusCode = (int)statusCode;

                    // Return a problem details object.
                    // Read more: https://www.rfc-editor.org/rfc/rfc7807
                    //        and https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails
                    var pd = new ProblemDetails
                    {
                        Type = "https://http.cat/400",
                        Title = "An error occurred while processing your request.",
                        Status = (int)statusCode,
                        Detail = ex.Message
                    };
                    await context.Response.WriteAsJsonAsync(pd);
                }
            }));
    }
}
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Registration;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Handling request of type {RequestType}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("Handled request of type {RequestType} in {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
        return response;
    }
}

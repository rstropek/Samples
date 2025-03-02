using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Concurrent;
using System.Reflection;

namespace Registration;

public class ValidationErrors(IEnumerable<ValidationFailure> errors) : Error
{
    public IEnumerable<ValidationFailure> Errors { get; } = errors;
}

public class ValidationResultBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> _failMethodCache = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationTasks = validators.Select(v => v.ValidateAsync(context, cancellationToken)).ToArray();
            var validationResults = await Task.WhenAll(validationTasks);
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).Distinct().ToList();
            if (failures.Count != 0)
            {
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var validationErrors = new ValidationErrors(failures);
                    var resultType = typeof(TResponse);
                    var genericArgType = resultType.GetGenericArguments()[0];
                    
                    // Get or create the generic fail method from cache
                    var genericFailMethod = _failMethodCache.GetOrAdd(genericArgType, type => {
                        var failMethods = typeof(Result).GetMethods()
                            .Where(m => m.Name == nameof(Result.Fail) &&
                                    m.IsGenericMethod &&
                                    m.GetParameters().Length == 1 &&
                                    m.GetParameters()[0].ParameterType == typeof(IError))
                            .First();
                        return failMethods.MakeGenericMethod(type);
                    });
                    
                    var result = genericFailMethod.Invoke(null, [validationErrors]);
                    return (TResponse)result!;
                }

                // If not a Result<T>, throw exception as before
                throw new ValidationException(failures);
            }
        }

        var response = await next();
        return response;
    }
}

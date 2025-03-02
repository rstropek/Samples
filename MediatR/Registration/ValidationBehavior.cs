using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Concurrent;
using System.Reflection;

namespace Registration;

/// <summary>
/// Helper class to convert a list of validation failures into a FluentResults error.
/// </summary>
/// <param name="errors">The list of validation failures.</param>
public class ValidationErrors(IEnumerable<ValidationFailure> errors) : Error
{
    public IEnumerable<ValidationFailure> Errors { get; } = errors;
}

/// <summary>
/// Pipeline behavior to validate the request and return a FluentResults error if there are validation failures.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="validators">The list of validators to use.</param>
public class ValidationResultBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Cache of the fail method for the Result type.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo> failMethodCache = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Do we have any validators?
        // We use DI to get the validators (https://docs.fluentvalidation.net/en/latest/di.html#automatic-registration)
        if (validators.Any())
        {
            // Validate the request and get the validation results
            var context = new ValidationContext<TRequest>(request);
            var validationTasks = validators.Select(v => v.ValidateAsync(context, cancellationToken)).ToArray();
            var validationResults = await Task.WhenAll(validationTasks);
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).Distinct().ToList();

            // Are there any validation failures?
            if (failures.Count != 0)
            {
                // If the response is a Result<T>, Result.Fail() can be used.
                // In this case, we do NOT throw an exception.
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var validationErrors = new ValidationErrors(failures);
                    var resultType = typeof(TResponse);
                    var genericArgType = resultType.GetGenericArguments()[0];
                    
                    // Get or create the generic fail method from cache
                    var genericFailMethod = failMethodCache.GetOrAdd(genericArgType, type => {
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

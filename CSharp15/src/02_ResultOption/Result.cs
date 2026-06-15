// ============================================================================
//  Result<T, E> — a union modelling "success T, or failure E".
//  Demonstrates a union with *two* type parameters + extension blocks.
// ============================================================================
namespace Functional;

public record class Ok<T>(T Value);
public record class Error<E>(E Failure);

public union Result<T, E>(Ok<T>, Error<E>);

public static class Result
{
    public static Result<T, E> Ok<T, E>(T value) => new Ok<T>(value);
    public static Result<T, E> Fail<T, E>(E error) => new Error<E>(error);
}

// Extension block over Result<T, E>. Unlike Option, a Result has no natural
// "empty" value, so the `null` arm (an uninitialised `default(Result<,>)`)
// is a programming error and throws.
public static class ResultExtensions
{
    extension<T, E>(Result<T, E> result)
    {
        public bool IsOk => result is Ok<T>;

        public Result<U, E> Map<U>(Func<T, U> f) => result switch
        {
            Ok<T> ok   => new Ok<U>(f(ok.Value)),
            Error<E> e => e,                      // Error<E> -> Result<U,E>
            null       => throw Uninitialised(),
        };

        public Result<U, E> Bind<U>(Func<T, Result<U, E>> f) => result switch
        {
            Ok<T> ok   => f(ok.Value),
            Error<E> e => e,
            null       => throw Uninitialised(),
        };

        public R Match<R>(Func<T, R> ok, Func<E, R> error) => result switch
        {
            Ok<T> o    => ok(o.Value),
            Error<E> e => error(e.Failure),
            null       => throw Uninitialised(),
        };
    }

    private static InvalidOperationException Uninitialised() =>
        new("Result was used uninitialised (default). Construct it from Ok/Error.");
}

namespace Functional;

/// <summary>Success value or error message.</summary>
public sealed class Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    private Result(T? value, string? error, bool isSuccess)
    {
        _value = value;
        _error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException($"Result is a failure: {_error}");
    public string Error => !IsSuccess ? _error! : throw new InvalidOperationException("Result is a success");

    public static Result<T> Success(T value) => new(value, null, true);
    public static Result<T> Failure(string error) => new(default, error, false);

    public static implicit operator Result<T>(T value) => Success(value);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);

    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        IsSuccess ? Result<TOut>.Success(map(_value!)) : Result<TOut>.Failure(_error!);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) =>
        IsSuccess ? bind(_value!) : Result<TOut>.Failure(_error!);

    public override string ToString() => IsSuccess ? $"Success({_value})" : $"Failure({_error})";
}

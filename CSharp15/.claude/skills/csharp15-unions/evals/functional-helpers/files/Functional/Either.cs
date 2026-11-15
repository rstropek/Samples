namespace Functional;

/// <summary>A value that is either a <typeparamref name="T0"/> or a <typeparamref name="T1"/>
/// (modelled after the OneOf library, which we did not want as a dependency).</summary>
public readonly struct Either<T0, T1>
{
    private readonly object? _value;
    private readonly int _index;

    private Either(object? value, int index) { _value = value; _index = index; }

    public bool IsT0 => _index == 0;
    public bool IsT1 => _index == 1;
    public T0 AsT0 => _index == 0 ? (T0)_value! : throw new InvalidOperationException("Not a T0");
    public T1 AsT1 => _index == 1 ? (T1)_value! : throw new InvalidOperationException("Not a T1");

    public static implicit operator Either<T0, T1>(T0 value) => new(value, 0);
    public static implicit operator Either<T0, T1>(T1 value) => new(value, 1);

    public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1) => _index switch
    {
        0 => f0((T0)_value!),
        1 => f1((T1)_value!),
        _ => throw new InvalidOperationException("Uninitialised Either"),
    };

    public void Switch(Action<T0> a0, Action<T1> a1)
    {
        if (_index == 0) a0((T0)_value!);
        else if (_index == 1) a1((T1)_value!);
        else throw new InvalidOperationException("Uninitialised Either");
    }
}

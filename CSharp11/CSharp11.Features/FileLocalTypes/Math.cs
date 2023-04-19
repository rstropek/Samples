using System.Numerics;

public class Math
{
    private readonly Calculator<int> calc = new();

    public int Add(int x, int y) => calc.Add(x, y);
}

// Note file-local type. Use dnSpy to check what this becomes.
file class Calculator<T> where T: IAdditionOperators<T, T, T>
{
    public T Add(T x, T y) => x + y;
}

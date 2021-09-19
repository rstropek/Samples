namespace GenericMath;
using Xunit;

public record struct Vector2d<T>(T X, T Y)
    : IAdditionOperators<Vector2d<T>, Vector2d<T>, Vector2d<T>>,
      IAdditionOperators<Vector2d<T>, T, Vector2d<T>>
    where T : INumber<T>
{
    public static Vector2d<T> operator +(Vector2d<T> left, Vector2d<T> right)
        => new(left.X + right.X, left.Y + right.Y);

    public static Vector2d<T> operator +(Vector2d<T> left, T delta)
        => new(left.X + delta, left.Y + delta);
}

public class GenericMath
{
    [Fact]
    public void VectorAdd()
    {
        var v1 = new Vector2d<int>(1, 1);
        var v2 = v1 + new Vector2d<int>(2, 2);
        Assert.Equal(new Vector2d<int>(3, 3), v2);
    }

    [Fact]
    public void VectorValueAdd()
    {
        var v1 = new Vector2d<int>(1, 1);
        var v2 = v1 + 2;
        Assert.Equal(new Vector2d<int>(3, 3), v2);
    }

    [Fact]
    public void VectorSum()
    {
        var vs = new[] { new Vector2d<int>(1, 1), new Vector2d<int>(2, 2) };
        var sum = new Vector2d<int>(0, 0);
        foreach (var v in vs)
        {
            sum += v;
        }

        Assert.Equal(new Vector2d<int>(3, 3), sum);
    }
}

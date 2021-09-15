namespace CSharpImmutables.Tests;

public class ImmutableStructsTests
{
    [Fact]
    public void MutableVector2d()
    {
        var v = new MutableVector2d();
        Assert.Equal(new(0d, 0d), v);

        v = new MutableVector2d(1d, 1d);
        Assert.Equal(new(1d, 1d), v);

        v = new MutableVector2d { X = 1d, Y = 1d, };
        Assert.Equal(new(1d, 1d), v);

        v = new MutableVector2d(1d, 1d);
        v.Double();
        Assert.Equal(new(2d, 2d), v);

        Assert.Equal(Math.Sqrt(2d), new MutableVector2d(1d, 1d).Distance);
    }

    [Fact]
    public void ImmutableVector2d()
    {
        var v = new ImmutableVector2d();
        Assert.Equal(new(0d, 0d), v);

        v = new ImmutableVector2d(1d, 1d);
        Assert.Equal(new(1d, 1d), v);

        v = new ImmutableVector2d { X = 1d, Y = 1d, };
        Assert.Equal(new(1d, 1d), v);

        v = new ImmutableVector2d(1d, 1d);
        v = v.Double();
        Assert.Equal(new(2d, 2d), v);

        Assert.Equal(Math.Sqrt(2d), new ImmutableVector2d(1d, 1d).Distance);
    }
}

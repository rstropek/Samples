namespace RecordsInsideOut;

public class RecordStructs
{
    #region Value semantics
    record struct Vector2d(double X, double Y)
    {
        // Note that translate can manipulate object in-place as it is not readonly
        public void Translate(double dx, double dy) => (X, Y) = (X + dx, Y + dy);
    }

    [Fact]
    public void ValueSemantics()
    {
        var v1 = new Vector2d(1d, 1d);
        var v2 = v1;
        v1.X = 2d;
        Assert.Equal(1d, v2.X);

        // Because it is a vector, we can use stackalloc with it
        Span<Vector2d> vectors = stackalloc Vector2d[] { new(1d, 1d), new(2d, 2d) };
        Assert.Equal(2, vectors.Length);
    }
    #endregion

    #region Optional immutability
    readonly record struct ImmutableVector2d(double X, double Y)
    {
        // Note that we need to return a new object as object is immutable
        public ImmutableVector2d Translate(double dx, double dy) => this with { X = X + dx, Y = Y + dy };
    }

    [Fact]
    public void Immutability()
    {
        // Mutable record struct
        var v1 = new Vector2d(1d, 2d);
        v1.X += 1d; // Records are not immutable by default
        Assert.Equal(2d, v1.X);

        v1.Translate(1d, 1d); // Manipulates object in-place
        Assert.Equal(new(3d, 3d), v1);

        // Immutable record struct
        var v2 = new ImmutableVector2d(1d, 2d);
        //v2.X += 1d; // -> doesn't work as this is a readyonly record struct
        Assert.Equal(new(2d, 3d), v2.Translate(1d, 1d));
    }
    #endregion

    #region Hashcodes
    [Fact]
    public void Hashcode()
    {
        // Hashcodes are identical
        Assert.Equal(new ImmutableVector2d(1d, 1d).GetHashCode(),
            new ImmutableVector2d(1d, 1d).GetHashCode());

        // Readonly record can be used e.g. as dict key
        var interestingLocations = new Dictionary<ImmutableVector2d, string>
        {
            [new(42d, 42d)] = "Magratheans",
            [new(3.1415d, 3.1415d)] = "PI",
        };
        Assert.Equal("Magratheans", interestingLocations[new(42d, 42d)]);
    }
    #endregion
}

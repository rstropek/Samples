using Functional;

namespace Csharp15.Tests;

public class OptionTests
{
    [Fact]
    public void Some_carries_value()
    {
        Option<int> o = Option.Some(42);
        Assert.True(o.IsSome);
        Assert.Equal(42, o.GetValueOrDefault(-1));
    }

    [Fact]
    public void None_has_no_value()
    {
        Option<int> o = Option.None<int>();
        Assert.False(o.IsSome);
        Assert.Equal(-1, o.GetValueOrDefault(-1));
    }

    [Fact]
    public void Map_transforms_Some_and_passes_through_None()
    {
        Assert.Equal(6, Option.Some(3).Map(x => x * 2).GetValueOrDefault(0));
        Assert.Equal(0, Option.None<int>().Map(x => x * 2).GetValueOrDefault(0));
    }

    [Fact]
    public void Bind_chains_optional_computations()
    {
        Option<int> Half(int x) => x % 2 == 0 ? Option.Some(x / 2) : Option.None<int>();

        Assert.Equal(4, Option.Some(8).Bind(Half).GetValueOrDefault(-1));
        Assert.Equal(-1, Option.Some(7).Bind(Half).GetValueOrDefault(-1));
    }

    [Fact]
    public void Of_maps_null_to_None()
    {
        Assert.False(Option.Of<string>(null).IsSome);
        Assert.True(Option.Of("hi").IsSome);
    }

    [Fact]
    public void Match_is_exhaustive()
    {
        string Render(Option<int> o) => o.Match(v => $"some {v}", () => "none");
        Assert.Equal("some 5", Render(Option.Some(5)));
        Assert.Equal("none", Render(Option.None<int>()));
    }
}

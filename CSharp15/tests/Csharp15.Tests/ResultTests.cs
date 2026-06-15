using Functional;

namespace Csharp15.Tests;

public class ResultTests
{
    private static Result<int, string> Parse(string s) =>
        int.TryParse(s, out var n)
            ? Result.Ok<int, string>(n)
            : Result.Fail<int, string>($"'{s}' is not an integer");

    [Fact]
    public void Ok_path_matches_and_maps()
    {
        var r = Parse("21").Map(x => x * 2);
        Assert.Equal("ok 42", r.Match(v => $"ok {v}", e => $"err {e}"));
    }

    [Fact]
    public void Error_short_circuits_Map()
    {
        var r = Parse("oops").Map(x => x * 2);
        Assert.Equal("err 'oops' is not an integer", r.Match(v => $"ok {v}", e => $"err {e}"));
    }

    [Fact]
    public void Bind_chains_results()
    {
        Result<int, string> Reciprocal(int x) =>
            x == 0 ? Result.Fail<int, string>("divide by zero") : Result.Ok<int, string>(100 / x);

        Assert.Equal(25, Parse("4").Bind(Reciprocal).Match(v => v, _ => -1));
        Assert.Equal(-1, Parse("0").Bind(Reciprocal).Match(v => v, _ => -1));
    }

    [Fact]
    public void Implicit_conversion_from_case_type()
    {
        // Ok<int> converts implicitly into Result<int, string>
        Result<int, string> r = new Ok<int>(7);
        Assert.Equal(7, r.Match(v => v, _ => -1));
    }
}

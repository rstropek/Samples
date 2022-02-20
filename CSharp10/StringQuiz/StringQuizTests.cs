using System.Text;
using Xunit;

namespace StringQuiz;

public class StringQuizTests
{
    [Fact]
    public void StaticStrings()
    {
        const string Greeting = "Hello";
        const string Name = "BASTA";
        string result = $"{Greeting}, {Name}!";
        Assert.Equal("Hello, BASTA!", result);
    }

    [Fact]
    public void ConcatWithInterpolation()
    {
        static string GreetingBuilder(string greeting, string name)
            => $"{greeting}, {name}!";

        string result = GreetingBuilder("Hello", "BASTA");
        Assert.Equal("Hello, BASTA!", result);
    }

    [Fact]
    public void FormatString()
    {
        var today = new DateTime(2022, 1, 1); //DateTime.Today;
        var status = $@"Today is {today:MMMM}, {today.Day}{
            today.Day switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            }
        } {today.Year}";
        Assert.Equal("Today is January, 1st 2022", status);
    }

    [Fact]
    public void Interpolation()
    {
        static string FormatVersion(int major, int minor, int build, int revision)
            => $"{major}.{minor}.{build}.{revision}";

        string result = FormatVersion(1, 2, 3, 4);
        Assert.Equal("1.2.3.4", result);
    }

    [Fact]
    public void InterpolateIntoSpan()
    {
        static string FormatVersion(int major, int minor, int build, int revision)
            => string.Create(null, stackalloc char[64], $"{major}.{minor}.{build}.{revision}");

        string result = FormatVersion(1, 2, 3, 4);
        Assert.Equal("1.2.3.4", result);
    }

    record struct Point2d(int X, int Y)
    {
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => destination.TryWrite(null, $"{X}, {Y}", out charsWritten);
    }

    [Fact]
    public void InterpolateIntoSpan2()
    {
        var p = new Point2d(1, 2);
        Span<char> buf = stackalloc char[20];
        if (p.TryFormat(buf, out var charsWritten))
        {
            Assert.Equal("1, 2", buf.ToString()[..charsWritten]);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void StringBuilderAppend()
    {
        static void AppendVersion(StringBuilder builder, int major, int minor, int build, int revision)
            => builder.Append($"{major}.{minor}.{build}.{revision}");

        var sb = new StringBuilder();
        AppendVersion(sb, 1, 2, 3, 4);
        Assert.Equal("1.2.3.4", sb.ToString());
    }
}

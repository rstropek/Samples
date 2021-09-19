namespace DateTimeImprovements;
using Xunit;

public class TimeOnlyTests
{
    [Fact]
    public void Creation()
    {
        _ = new TimeOnly(17, 30, 42, 999);

        // Note: TimeOnly is internally an immutable long (can be read using Ticks property)
        // representing 100-nanosecond intervals since beginning of day
        // https://source.dot.net/#System.Private.CoreLib/TimeOnly.cs
        Assert.Equal(0, TimeOnly.MinValue.Ticks);
    }

    [Fact]
    public void Parsing()
    {
        _ = TimeOnly.Parse("17:30:42.999");
        _ = DateOnly.ParseExact("17:30:42.999", @"HH\:mm\:ss\.fff");

        // Note: Support for span parsing
        _ = TimeOnly.Parse("17:30:42.999".AsSpan());

        Assert.True(TimeOnly.TryParse("17:30:42.999", out var _));

        // The following line does not at the time of writing. No support
        // for DateOnly/TimeOnly in JsonSerializer yet. Read more at
        // https://github.com/dotnet/runtime/issues/53539
        // _ = JsonSerializer.Deserialize<TimeOnly>("\"17:30:42.999\"");
    }

    [Fact]
    public void Conversion()
    {
        var time = TimeOnly.FromTimeSpan(TimeSpan.FromHours(1));
        _ = TimeOnly.FromDateTime(DateTime.Now);

        TimeSpan timespan = time.ToTimeSpan();
        Assert.Equal(time.Ticks, timespan.Ticks);

        Assert.Equal("01:00:00", time.ToString(@"HH\:mm\:ss"));
    }

    [Fact]
    public void Manipulation()
    {
        TimeOnly time = new(17, 30, 42, 999);

        var nextMinute = time.AddMinutes(1);
        Assert.Equal(new TimeOnly(17, 31, 42, 999), nextMinute);

        var nextHour = time.AddHours(1);
        Assert.Equal(new TimeOnly(18, 30, 42, 999), nextHour);

        var nextSecond = time.Add(TimeSpan.FromSeconds(1));
        Assert.Equal(new TimeOnly(17, 30, 43, 999), nextSecond);
    }

    [Fact]
    public void Properties()
    {
        TimeOnly time = new(17, 30, 42, 999);

        Assert.Equal(17, time.Hour);
        Assert.Equal(30, time.Minute);
        Assert.Equal(42, time.Second);
        Assert.Equal(999, time.Millisecond);
        Assert.Equal(630429990000, time.Ticks);

        Assert.Equal(TimeOnly.MinValue, new TimeOnly(0, 0));
    }
}

using System;
using System.Text.Json;
using Xunit;

namespace DateTimeImprovements
{
    public class DateOnlyTests
    {
        [Fact]
        public void Creation()
        {
            _ = new DateOnly(2021, 7, 25);

            // Note: DateOnly is internally an immutable int (can be read using DayNumber property)
            // representing days since Jan. 1st 0001. For details see implementation at
            // https://source.dot.net/#System.Private.CoreLib/DateOnly.cs
            Assert.Equal(new DateOnly(1, 1, 1), DateOnly.FromDayNumber(0));
        }

        [Fact]
        public void Parsing()
        {
            _ = DateOnly.Parse("2021-07-25");
            _ = DateOnly.ParseExact("2021-07-25", "yyyy-MM-dd");

            // Note: Support for span parsing
            _ = DateOnly.Parse("2021-07-25".AsSpan());

            Assert.True(DateOnly.TryParse("2021-07-25", out var _));

            // The following line does not at the time of writing. No support
            // for DateOnly/TimeOnly in JsonSerializer yet. Read more at
            // https://github.com/dotnet/runtime/issues/53539
            // _ = JsonSerializer.Deserialize<DateOnly>("\"2021-07-25\"");
        }

        [Fact]
        public void Conversion()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);

            DateTime dateTime = date.ToDateTime(TimeOnly.MinValue);
            Assert.Equal(date.DayOfYear, dateTime.DayOfYear);

            Assert.Equal("2021-07-25", new DateOnly(2021, 7, 25).ToString("o")); // (ISO 8601 format)
        }

        [Fact]
        public void Manipulation()
        {
            DateOnly date = new(2021, 1, 31);

            var nextDate = date.AddDays(1);
            Assert.Equal(new DateOnly(2021, 2, 1), nextDate);

            var nextMonth = date.AddMonths(1);
            Assert.Equal(new DateOnly(2021, 2, 28), nextMonth);
        }

        [Fact]
        public void Properties()
        {
            DateOnly date = new(2021, 7, 25);

            Assert.Equal(2021, date.Year);
            Assert.Equal(7, date.Month);
            Assert.Equal(25, date.Day);
            Assert.Equal(DayOfWeek.Sunday, date.DayOfWeek);
            Assert.Equal(737995, date.DayNumber);

            Assert.Equal(DateOnly.MinValue, new DateOnly(1, 1, 1));
            Assert.Equal(DateOnly.MaxValue, new DateOnly(9999, 12, 31));
        }
    }
}

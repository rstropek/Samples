using System.Collections.Frozen;

namespace Lists;

// NOTE: See also Benchmark project in ../ListsBenchmarks

public class FrozenCollections
{
    [Fact]
    public void FrozenSet()
    {
        // Create a frozen set from any enumerable. Note that
        // we do not need to create it from a HashSet (important
        // as creating a hash set is expensive).
        var numbersEnumerable = Enumerable.Range(0, 100_000);
        var numbers = numbersEnumerable.ToFrozenSet();

        // Frozen sets are immutable, so we can't add or remove
        // items from them. They are "more" immutable than
        // ImmutableHashSet<T> as they don't even have methods
        // that return a new set with an item added or removed.

        Assert.Contains(42, numbers.AsEnumerable());

        // We can use set operations.
        Assert.True(numbers.IsSupersetOf([1, 2, 3, 3]));
        Assert.False(numbers.IsProperSupersetOf(Enumerable.Range(0, 100_000)));
        Assert.False(numbers.IsSupersetOf([-2, -1, 0, 1]));
        Assert.True(numbers.Overlaps([-2, -1, 0, 1]));
    }

    [Fact]
    public void FrozenDictionary()
    {
        // Create a frozen dictionary from any enumerable. Note that
        // we do not need to create it from a Dictionary (similar to
        // frozen sets).
        var numbers = Enumerable.Range(0, 1000)
            .Select(i => (i, written: NumberToWrittenForm(i)))
            .ToFrozenDictionary(pair => pair.i, pair => pair.written);

        // We have the usual dictionary operations.
        Assert.Equal("fortytwo", numbers[42]);
        Assert.True(numbers.ContainsKey(42));
        Assert.False(numbers.ContainsKey(1001));
        Assert.True(numbers.TryGetValue(123, out var ohtt));
        Assert.Equal("onehundredtwentythree", ohtt);

        // We can iterate over keys and values.
        Assert.Equal(1000, numbers.Keys.Length);
        Assert.Equal(1000, numbers.Values.Length);
    }

    public static string NumberToWrittenForm(int value)
    {
        if (value is < 0 or > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        var writtenForms = new Dictionary<int, string>
        {
            [0] = "zero",
            [1] = "one",
            [2] = "two",
            [3] = "three",
            [4] = "four",
            [5] = "five",
            [6] = "six",
            [7] = "seven",
            [8] = "eight",
            [9] = "nine",
            [10] = "ten",
            [11] = "eleven",
            [12] = "twelve",
            [13] = "thirteen",
            [14] = "fourteen",
            [15] = "fifteen",
            [16] = "sixteen",
            [17] = "seventeen",
            [18] = "eighteen",
            [19] = "nineteen",
            [20] = "twenty",
            [30] = "thirty",
            [40] = "forty",
            [50] = "fifty",
            [60] = "sixty",
            [70] = "seventy",
            [80] = "eighty",
            [90] = "ninety",
            [100] = "hundred",
            [1000] = "thousand"
        };

        if (writtenForms.TryGetValue(value, out var writtenForm))
        {
            return writtenForm;
        }

        if (value < 100)
        {
            var tens = value / 10 * 10;
            var ones = value % 10;
            return $"{NumberToWrittenForm(tens)}{NumberToWrittenForm(ones)}";
        }

        if (value < 1000)
        {
            var hundreds = value / 100;
            var tens = value % 100;
            return $"{NumberToWrittenForm(hundreds)}{NumberToWrittenForm(100)}{NumberToWrittenForm(tens)}";
        }

        throw new NotImplementedException();
    }
}
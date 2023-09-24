using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text;
using BenchmarkDotNet.Attributes;

/* Result (on my laptop):

| Method                              | Mean     | Error    | StdDev   |
|------------------------------------ |---------:|---------:|---------:|
| CheckIfHaikuExists_HashSet          | 21.64 ns | 0.403 ns | 0.377 ns |
| CheckIfHaikuExists_ImmutableHashSet | 51.51 ns | 1.056 ns | 2.180 ns |
| CheckIfHaikuExists_FrozenSet        | 17.25 ns | 0.362 ns | 0.671 ns |

| Method                          | Mean     | Error     | StdDev    |
|-------------------------------- |---------:|----------:|----------:|
| GetWrittenForm_FrozenDictionary | 4.031 ns | 0.1277 ns | 0.1311 ns |
| GetWrittenForm_Dictionary       | 5.055 ns | 0.1522 ns | 0.1925 ns |
*/

//BenchmarkDotNet.Running.BenchmarkRunner.Run<FrozenSets>();
BenchmarkDotNet.Running.BenchmarkRunner.Run<FrozenDictionaries>();

public class FrozenSets
{
    static readonly string[] haikuLines = {
        "Autumn leaves falling",
        "Whispers of a distant breeze",
        "Nature's quiet song",
        "Cherry blossoms bloom",
        "Petals fall like gentle rain",
        "Spring's fleeting beauty",
        "Moonlight on the lake",
        "Reflecting stars in stillness",
        "Night's tranquil embrace"
    };

    private static HashSet<string>? hashSet;
    private static ImmutableHashSet<string>? immutableHashSet;
    private static FrozenSet<string>? frozenSet;
    private static readonly string haikuToFind = GenerateHaiku();

    private static string GenerateHaiku()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 3; i++)
        {
            sb.AppendLine(haikuLines[Random.Shared.Next(haikuLines.Length)]);
        }
        
        return sb.ToString();
    }

    [GlobalSetup]
    public void Setup()
    {
        hashSet = new(Enumerable.Range(0, 100_000).Select(_ => GenerateHaiku()));
        immutableHashSet = [.. hashSet];
        frozenSet = hashSet.ToFrozenSet();
    }

    [Benchmark]
    public bool CheckIfHaikuExists_HashSet() => hashSet!.Contains(haikuToFind);

    [Benchmark]
    public bool CheckIfHaikuExists_ImmutableHashSet() => immutableHashSet!.Contains(haikuToFind);

    [Benchmark]
    public bool CheckIfHaikuExists_FrozenSet() => frozenSet!.Contains(haikuToFind);
}

public class FrozenDictionaries
{
    private static string NumberToWrittenForm(int value)
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

    private static FrozenDictionary<int, string>? frozenDictionary;

    private static Dictionary<int, string>? dictionary;

    [GlobalSetup]
    public void Setup()
    {
        frozenDictionary = Enumerable.Range(0, 1001)
            .Select(i => (i, written: NumberToWrittenForm(i)))
            .ToFrozenDictionary(pair => pair.i, pair => pair.written);

        dictionary = Enumerable.Range(0, 1001)
            .Select(i => (i, written: NumberToWrittenForm(i)))
            .ToDictionary(pair => pair.i, pair => pair.written);
    }

    [Benchmark]
    public string? GetWrittenForm_FrozenDictionary() => frozenDictionary![Random.Shared.Next(1001)];

    [Benchmark]
    public string? GetWrittenForm_Dictionary() => dictionary![Random.Shared.Next(1001)];
}
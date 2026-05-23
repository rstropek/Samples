using System.Runtime.CompilerServices;

// .NET 8/C# 12
{
    // No new LINQ operators, but some new overloads for existing ones.
    // https://github.com/dotnet/core/blob/main/release-notes/8.0/8.0.0/api-diff/Microsoft.NETCore.App/8.0.0_System.Linq.md

    var kvps = new[]
    {
        new KeyValuePair<string, int>("one", 1),
        new KeyValuePair<string, int>("two", 2),
    };
    Dictionary<string, int> map = kvps.ToDictionary();

    var tuples = new (string Key, string Value)[]
    {
        ("AT", "Austria"),
        ("DE", "Germany"),
    };
    Dictionary<string,string> countries = tuples.ToDictionary();

    var tuples2 = new[] { ("alpha", 1), ("beta", 2) };
    var ci = tuples2.ToDictionary(StringComparer.OrdinalIgnoreCase);
    // lookups ignore case:
    Console.WriteLine(ci.ContainsKey("ALPHA")); // True
}

// .NET 9/C# 13
{
    // https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview?utm_source=chatgpt.com#net-libraries

    // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.index
    string[] animals = ["cat", "dog", "yak"];
    foreach (var (index, item) in animals.Select((item, index) => (index, item)))
    {
        Console.WriteLine($"{index}: {item}");
    }
    foreach (var (index, item) in animals.Index())
    {
        Console.WriteLine($"{index}: {item}");
    }

    // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.countby
    var words = new[] { "a", "bb", "ccc", "dd", "e" };
    var oldCountsByLength = words
        .GroupBy(w => w.Length)
        .ToDictionary(g => g.Key, g => g.Count());
    foreach (var kv in oldCountsByLength)
    {
        Console.WriteLine($"{kv.Key}-letter words: {kv.Value}");
    }

    var countsByLength = words.CountBy(w => w.Length);
    foreach (var kv in countsByLength)
    {
        Console.WriteLine($"{kv.Key}-letter words: {kv.Value}");
    }

    // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.aggregateby
    var sales = new[]
    {
        new { Category = "Food",  Amount = 10m },
        new { Category = "Food",  Amount =  5m },
        new { Category = "Books", Amount = 20m },
    };
    var totals = sales.AggregateBy(keySelector: s => s.Category, seed: 0m, (acc, s) => acc + s.Amount);
    foreach (var kv in totals)
    {
        Console.WriteLine($"{kv.Key}: {kv.Value:C}");
    }
    // Food: 15.00
    // Books: 20.00

}

// .NET 10/C# 14
{
    // Past: https://www.nuget.org/packages/System.Linq.Async/6.0.3#readme-body-tab
    // Now: Framework-provided

    // Create a throwaway sample file so the demo is runnable as-is.
    string path = Path.Combine(Path.GetTempPath(), "async-linq-demo.txt");
    await File.WriteAllLinesAsync(path,
    [
        "# comment line",
        " hello world ",
        "",
        "This is a test.",
        "  Another   line  ",
        "# another comment",
        "C# and .NET are fun!"
    ]);

    using var cts = new CancellationTokenSource();

    // Async LINQ over LINES
    var cleanedNonCommentLines = ReadLinesAsync(path, cts.Token)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Where(line => !line.TrimStart().StartsWith('#'))
        .Select(line => line.Trim())
        .Take(5);
    var linesList = await cleanedNonCommentLines.ToListAsync(cts.Token);
    Console.WriteLine("Cleaned (first 5) lines:");
    foreach (var l in linesList)
    {
        Console.WriteLine($"- {l}");
    }

    var totalChars = await ReadLinesAsync(path, cts.Token)
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Select(l => l.Length)
        .SumAsync(cts.Token);
    Console.WriteLine($"\nTotal chars in non-empty lines: {totalChars}");

    // Separate async iterator: read the file LINE-BY-LINE.
    // Note: This is DEMO ONLY! ReadLineAsync(CancellationToken) is available in modern runtimes.
    static async IAsyncEnumerable<string> ReadLinesAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(File.OpenRead(path));
        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null) { yield break; }
            yield return line;
        }
    }
}
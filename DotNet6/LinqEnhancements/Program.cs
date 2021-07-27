#nullable enable

using static System.Console;
using System.Linq;
using System.Collections.Generic;

var heroes = new Hero[]
{
    new("Homelander", "John", 9, 9),
    new("Queen Maeve", "Maggie Shaw", 9, 5),
    new("Black Noir", null, 9, 7),
    new("A-Train", "Regie Franklin", 7, 8),
    new("The Deep", "Kevin Moskowitz", 3, 7),
    new("Translucent", null, 5, 7),
    new("Lamplighter", null, 3, 2),
    new("Starlight", "Annie January", 7, 0),
    new("Stormfront", "Klara Risinger", 9, 9),
};

// Support for ranges in Take
var originalSeven = heroes.Take(..7).Select(h => h.Name);
WriteLine($"The original seven were {string.Join(',', originalSeven)}");

var joiners = heroes.Take(^2..).Select(h => h.Name);
WriteLine($"The seven were joined by {string.Join(',', joiners)}");

var leavers = heroes.Take(3..^2).Select(h => h.Name);
WriteLine($"At some point, the seven were left by {string.Join(',', leavers)}");

// New function TryGetNonEnumeratedCount to get count without enumerating
IEnumerable<Hero> GenerateHeroes(int? n = null)
{ 
    while (n is null || n-- > 0) yield return new("Dummy", "Foo Bar", 0, 0);
}

// Try the following two code variants and compare the output.
//var buffer = GenerateHeroes(10).ToArray().TryGetNonEnumeratedCount(out var count)
var buffer = GenerateHeroes().TryGetNonEnumeratedCount(out var count)
    ? new List<Hero>(capacity: count)
    : new List<Hero>();
WriteLine($"Capacity is {buffer.Capacity}");

// New optional key selectors for set operators like distinct, union, intersect, max, min, etc.
var coolnessExamples = heroes.DistinctBy(h => h.Coolness).Select(h => $"{h.Name} ({h.Coolness})");
WriteLine($"Examples for coolness levels: {string.Join(',', coolnessExamples)}");

var coolestExamples = heroes.MaxBy(h => h.Coolness);
WriteLine($"Example for hero with max coolness: { coolestExamples?.Name} ({ coolestExamples?.Coolness})");

// New chunk operator to split enumerable into chunks
foreach(var page in heroes.Chunk(4).Select((items, index) => (items, index)))
{
    WriteLine($"PAGE {page.index + 1}");
    foreach(var item in page.items)
    {
        WriteLine($"\t{item.Name}");
    }
}

// ...OrDefault functions with explicit default value
var notCoolAndNotEvil = heroes
    .FirstOrDefault(h => h.Coolness == 0 && h.Evilness == 0, new("Nobody", null, -1, -1));
WriteLine($"Hero who is not cool at all and absolutely not evil: {notCoolAndNotEvil.Name}");

// New Zip operator
var actors = new string[]
{
    "Antony Starr",
    "Dominique McElligott",
    "Nathan Mitchell",
    "Jessie T. Usher",
    "Chace Crawford",
    "Alex Hassell",
    "Shawn Ashmore",
    "Erin Moriarty",
    // "Aya Cash", // What happens if one enumerable is shorter than the other?
};

var heroActors = heroes.Select(h => h.Name).Zip(actors);
foreach(var ha in heroActors)
{
    WriteLine($"{ha.First} is played by {ha.Second}");
}

record Hero(string Name, string? RealName, int Coolness, int Evilness);

#nullable enable

using System;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Diagnostics;
using System.Linq;

var data = new { 
    Numbers = GetNumbers(5) // Note that GetNumbers return IAsyncEnumerable.
                            // Data to serialize streams into JsonSerializer.
                            // JSON serializer will turn it into an array of int.
};
await JsonSerializer.SerializeAsync(OpenStandardOutput(), data);
WriteLine();

Task<int> ReadFromSomewhereAsync() => Task.FromResult(new Random().Next(100));
async IAsyncEnumerable<int> GetNumbers(int n)
{
    for (int i = 0; i < n; i++)
    {
        yield return await ReadFromSomewhereAsync();
    }
}

// Note new API that returns IAsyncEnumerable from deserialized arrays.
// Deserialized data is "streaming" out of JsonSerializer
var stream = new MemoryStream(Encoding.UTF8.GetBytes("[0,1,1,2,3]"));
await foreach (int item in JsonSerializer.DeserializeAsyncEnumerable<int>(stream))
{
    Write($"{item} ");
}
WriteLine();

// Note new writeable DOM API
// Construct nodes by parsing string
var hero = JsonNode.Parse("{\"id\": 42, \"name\": \"Starlight\", \"canFly\": false}");
Debug.Assert((bool?)hero!["canFly"] is true);

var heroesToPatch = JsonNode.Parse(@"
{
    ""theBoys"": [
        { ""id"": 42, ""name"": ""Starlight"", ""canFly"": false },
        { ""id"": 43, ""name"": ""Homelander"", ""canFly"": true }
    ]
}
");
var theBoys = heroesToPatch!["theBoys"] as JsonArray;
var nameOfHero = theBoys![1]?["name"]?.GetValue<string?>();
WriteLine($"Name of hero is {nameOfHero}");
nameOfHero = (string?)theBoys!.First(b => b["id"]?.GetValue<int>() == 43)?["name"];
WriteLine($"Name of hero is {nameOfHero}");
theBoys!.RemoveAt(1);


var heroes = new JsonObject
{
    ["theBoys"] = new JsonArray(
        hero,
        JsonNode.Parse("{\"id\": 43, \"name\": \"Homelander\", \"canFly\": true}")
    )
};
WriteLine(heroes.ToJsonString());

dynamic dynamicHeroes = heroes;
WriteLine(dynamicHeroes.theBoys[0].name);

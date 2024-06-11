using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

// ==================================================================================================
// READ MORE ABOUT INTERCEPTORS AT
// https://devblogs.microsoft.com/dotnet/new-csharp-12-preview-features/#interceptors
// ==================================================================================================

#region JSON demo data
const string HeroesJsonReflection = """
    [
        { "Name": "Batman", "RealName": "Bruce Wayne", "CanFly": false },
        { "Name": "Cat Woman", "RealName": "Selina Kyle", "CanFly": false }
    ]
    """;
const string HeroesJsonSourceGen = """
    [
        { "Name": "Storm", "RealName": "Ororo Munroe", "CanFly": true },
        { "Name": "Magneto", "RealName": "Max Eisenhardt", "CanFly": true }
    ]
    """;
#endregion

var hm = new HeroesManager();
Console.WriteLine(string.Join(',', hm.GetHeroes(HeroesJsonReflection).Select(h => h.Name)));
//                                    +--- We will intercept this call
//                                    v
Console.WriteLine(string.Join(',', hm.GetHeroes(HeroesJsonSourceGen).Select(h => h.Name)));

class HeroesManager
{
    public Hero[] GetHeroes(string json)
    {
        // Deserialize JSON using regular reflection-based JsonSerializer
        var heroes = JsonSerializer.Deserialize<Hero[]>(json);
        Debug.Assert(heroes is not null);
        return heroes;
    }
}

namespace Interceptors
{
    // Note that this would typically be compiler-generated code
    [CompilerGenerated]
    static class HeroesmanagerAot
    {
        // Note how we can intercept a method call from generated code.
        [InterceptsLocation("/root/github/Samples/CSharp12/Interceptors/Program.cs", line: 29, character: 39)]
        public static Hero[] GetHeroesWithSourceGen(this HeroesManager hm, string json)
        {
            // Deserialize JSON using source-generated JsonSerializer
            var heroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.HeroArray);
            Debug.Assert(heroes is not null);
            return heroes;
        }
    }
}

record class Hero(string Name, string RealName, bool CanFly);

[JsonSerializable(typeof(Hero[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
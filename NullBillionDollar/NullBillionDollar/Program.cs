#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;

// Null check in functions
DoSomething("Hello");
static void DoSomething(string? s)
{
    ArgumentNullException.ThrowIfNull(s);
}

// Null coalescing operator
Console.WriteLine(IntoNotNull(null));
Console.WriteLine(MultipleIntoNotNull(null, "FooBar"));
static string IntoNotNull(string? maybeNullString) => maybeNullString ?? "default";
static string MultipleIntoNotNull(string? maybeNullString, string? second)
    => maybeNullString ?? second ?? "default";

// Null coalescing operator with throw expression
Console.WriteLine(ThrowIfNull("FooBar"));
static string ThrowIfNull(string? maybeNullString) => maybeNullString ?? throw new ArgumentNullException(null, nameof(maybeNullString));

ThrowIfNullStatement("FooBar");
static void ThrowIfNullStatement(string? maybeNullString) => _ = maybeNullString ?? throw new ArgumentNullException(null, nameof(maybeNullString));

// Null coalescing assignment operator
List<int>? numbers = null;
Push(42);
Push(43);
Console.WriteLine(JsonSerializer.Serialize(numbers));
void Push(int number)
{
    numbers ??= new List<int>();
    numbers.Add(number);
}

Console.WriteLine(GetNameOfHero(42));
Console.WriteLine(GetNameOfHero(44));
string GetNameOfHero(int id)
{
    var heroes = JsonNode.Parse("""
        {
            "theBoys": [
                { "id": 42, "name": "Starlight", "canFly": false },
                { "id": 43, "name": "Homelander", "canFly": true }
            ]
        }
        """);

    var theBoys = heroes?["theBoys"] as JsonArray;
    //                                     +---- Null conditional indexer
    //                                     |       +---- Null conditional operator
    //                                     |       |                  +---- Null coalescing operator
    //                                     v       v                  v
    return theBoys?.FirstOrDefault(b => ((b?["id"])?.GetValue<int?>() ?? -1) == id)?["name"]?.GetValue<string?>() ?? "not found";
}

Console.WriteLine(GetNameOfHeroForgiving(42));
Console.WriteLine(GetNameOfHeroForgiving(44));
string GetNameOfHeroForgiving(int id)
{
    var heroes = JsonNode.Parse("""
        {
            "theBoys": [
                { "id": 42, "name": "Starlight", "canFly": false },
                { "id": 43, "name": "Homelander", "canFly": true }
            ]
        }
        """);

    var theBoys = heroes!["theBoys"] as JsonArray;

    // Assumption: We KNOW that theBoys is not null and that properties id and name exist
    // -> We can use the ! operator to tell the compiler that we know what we are doing
    return theBoys!.FirstOrDefault(b => (b!["id"])!.GetValue<int>() == id)?["name"]!.GetValue<string?>() ?? "not found";
}

Console.WriteLine(NullPattern1(null));
string NullPattern1(string? input)
{
    // Note: Null pattern ignores overloads of == operator
    if (input is null)
    {
        return "default";
    }

    return input;
}

Console.WriteLine(NullPattern2(null));
string NullPattern2(string? input)
{
    if (input is not null)
    {
        return input;
    }

    return "default";
}
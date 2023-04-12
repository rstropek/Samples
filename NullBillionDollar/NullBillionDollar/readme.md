# Code Snippets

## The Problem

```cs
var p = Parse("FooBar");
if (string.IsNullOrEmpty(p.LastName))
{
    Console.WriteLine("Person does not have a last name");
}

/// <summary>
/// Parses person data
/// </summary>
/// <returns>
/// Person data or <c>null</c> if given string is invalid.
/// </returns>
static Person Parse(ReadOnlySpan<char> personData)
{
    var commaIx = personData.IndexOf(',');
    if (commaIx == -1) { return null; }

    return new Person(
        personData[..commaIx].ToString(),
        personData[(commaIx + 1)..].ToString());
}

record Person(string FirstName, string LastName);
```

```cs
static Person? Parse(ReadOnlySpan<char> personData)
{
    var commaIx = personData.IndexOf(',');
    if (commaIx == -1) { return null; }
    
    return new Person(
        personData[..commaIx].ToString(), 
        personData[(commaIx + 1)..].ToString());
}

readonly record struct Person(string FirstName, string LastName);
```

## How Other Languages Solve it

```rs
fn main() {
    let result = match get_maybe_null_string(2) {
        Some(s) => s,
        None => "Sorry, not string".to_string()
    };
    println!("{result}");

    let result = get_maybe_null_string(2);
    if let Some(s) = result {
        println!("We got a result and it is {s}");
    }
}

fn get_maybe_null_string(ix: i32) -> Option<String> {
    if ix % 2 == 0 { Some("FooBar".to_string()) } else { None }
}
```

## Intelligent Flow Analysis

```cs
#nullable enable

static string? GetHolidayName(DateOnly date)
{
    return date switch
    {
        { Day: 1, Month: 1 } => "New Year's Day",
        { Day: 7, Month: 4 } => "Independence Day",
        { Day: 25, Month: 12 } => "Christmas Day",
        var d when d == GaussEaster(date.Year) => "Easter",
        _ => null
    };
}

static string TranslateHolidayName(string holidayName)
{
    return holidayName switch {
        "New Year's Day" => "Neujahr",
        "Independence Day" => "Unabhängigkeitstag",
        "Christmas Day" => "Weihnachten",
        "Easter" => "Ostern",
        _ => throw new ArgumentException($"Unknown holiday name: {holidayName}")
   };
}

static DateOnly GaussEaster(int Y)
{
    float A, B, C, P, Q, M, N, D, E;

    A = Y % 19;
    B = Y % 4;
    C = Y % 7;
    P = (float)(int)(Y / 100);
    Q = (float)(int)((13 + 8 * P) / 25);
    M = (int)(15 - Q + P - (int)(P / 4)) % 30;
    N = (int)(4 + P - (int)(P / 4)) % 7;
    D = (19 * A + M) % 30;
    E = (2 * B + 4 * C + 6 * D + N) % 7;
    var days = (int)(22 + D + E);

    if ((D == 29) && (E == 6))
    {
        return new DateOnly(Y, 4, 19);
    }

    else if ((D == 28) && (E == 6))
    {
        return new DateOnly(Y, 4, 18);
    }
    else
    {
        if (days > 31)
        {
            return new DateOnly(Y, 4, days - 31);
        }
        else
        {
            return new DateOnly(Y, 3, days);
        }
    }
}
```

```cs
#nullable enable
using System.Diagnostics;

var holidayName = GetHolidayName(new(2023, 4, 9));

// ⚠️ The following line gives us a warning as holidayName might be null
Console.WriteLine(TranslateHolidayName(holidayName));

if (holidayName != null)
{
    // No warning as holidayName cannot be null (because of the if statement)
    Console.WriteLine(TranslateHolidayName(holidayName));
}

Debug.Assert(holidayName != null, "holidayName cannot be null");
// No warning as holidayName cannot be null (because of Debug.Assert)
Console.WriteLine(TranslateHolidayName(holidayName));

if (string.IsNullOrEmpty(holidayName))
{
    // No warning as holidayName cannot be null
    // (because of the IsNullOrEmpty inside the if statement)
    Console.WriteLine(TranslateHolidayName(holidayName));
}
```

## Sometimes C# Needs Help

```cs
#nullable enable

string? input = null;
const bool defaultValue = true;
var transformed = (input == null && defaultValue) ? "default" : input;
Console.WriteLine(transformed.Length);

var val = Transform(null, defaultValue);
Console.WriteLine(val.Length);

string? Transform(string? input, bool defaultValue)
{
    if (input == null && defaultValue) { return "default"; }
    return input;
}
```

## Null Coalescing

```cs
#nullable enable

// Null coalescing operator
Console.WriteLine(IntoNotNull(null));
Console.WriteLine(MultipleIntoNotNull(null, "FooBar"));
static string IntoNotNull(string? maybeNullString) => maybeNullString ?? "default";
static string MultipleIntoNotNull(string? maybeNullString, string? second)
    => maybeNullString ?? second ?? "default";

// Null coalescing operator with throw expression
Console.WriteLine(ThrowIfNull("FooBar"));
static string ThrowIfNull(string? maybeNullString) 
    => maybeNullString ?? throw new ArgumentNullException(null, nameof(maybeNullString));

ThrowIfNullStatement("FooBar");
static void ThrowIfNullStatement(string? maybeNullString) 
    => _ = maybeNullString ?? throw new ArgumentNullException(null, nameof(maybeNullString));
```

## Null Coalescing Assignment

```cs
#nullable enable

List<int>? numbers = null;
Push(42);
Push(43);
Console.WriteLine(JsonSerializer.Serialize(numbers));

void Push(int number)
{
    // Null coalescing assignment operator
    numbers ??= new List<int>();
    numbers.Add(number);
}
```

## Null Conditional

```cs
#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;

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
    return theBoys?.FirstOrDefault(b => ((b?["id"])?.GetValue<int?>() ?? -1) == id)?["name"]
        ?.GetValue<string?>() ?? "not found";
}
```

## Null Forgiving

```cs
#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;

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
    return theBoys!.FirstOrDefault(b => (b!["id"])!.GetValue<int>() == id)?["name"]
        !.GetValue<string?>() ?? "not found";
}
```

## Null Pattern

```cs
#nullable enable

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
```

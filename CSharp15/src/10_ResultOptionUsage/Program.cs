// ============================================================================
//  Using the Option<T> / Result<T,E> unions from 02_ResultOption in anger.
//  Two everyday scenarios:
//    (A) Option<T> for lookups that may legitimately find nothing.
//    (B) Result<T,E> for a validation pipeline (railway-oriented programming).
//  All combinators (Map/Bind/Match/IsSome/...) come from the extension blocks.
// ============================================================================
using Functional;

Console.WriteLine("== (A) Option: dictionary lookup ==");
var prices = new Dictionary<string, decimal>
{
    ["coffee"] = 3.50m,
    ["tea"]    = 2.75m,
};

foreach (var item in new[] { "coffee", "water" })
{
    // Lookup returns Option<decimal>; no nulls, no TryGetValue out-params leaking.
    string line = Lookup(prices, item)
        .Map(p => p * 1.2m)                                  // add 20% service
        .Match(p => $"{item,-7} -> {p:0.00}", () => $"{item,-7} -> not on the menu");
    Console.WriteLine(line);
}

Console.WriteLine();
Console.WriteLine("== (B) Result: validation pipeline ==");
foreach (var raw in new[] { "  alice@example.com / 30 ", "bob@ / 30", "carol@example.com / 999" })
{
    string outcome = ParseRegistration(raw)
        .Map(r => r with { Email = r.Email.ToLowerInvariant() })   // normalise
        .Match(
            ok:    r => $"OK   -> {r.Email}, age {r.Age}",
            error: e => $"FAIL -> {e}");
    Console.WriteLine($"{raw,-32} => {outcome}");
}

// ---- (A) helper -----------------------------------------------------------
static Option<decimal> Lookup(Dictionary<string, decimal> map, string key) =>
    map.TryGetValue(key, out var value) ? Option.Some(value) : Option.None<decimal>();

// ---- (B) helpers: each step returns Result and they chain with Bind -------
static Result<Registration, string> ParseRegistration(string raw)
{
    var parts = raw.Split('/', StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
        return Result.Fail<Registration, string>("expected 'email / age'");

    return ValidateEmail(parts[0])
        .Bind(email => ParseAge(parts[1])
            .Bind(age => ValidateAge(age)
                .Map(_ => new Registration(email, age))));
}

static Result<string, string> ValidateEmail(string email) =>
    email.Contains('@') && email.IndexOf('@') < email.Length - 1 && email.Contains('.')
        ? Result.Ok<string, string>(email)
        : Result.Fail<string, string>($"invalid email '{email}'");

static Result<int, string> ParseAge(string s) =>
    int.TryParse(s, out var age)
        ? Result.Ok<int, string>(age)
        : Result.Fail<int, string>($"age '{s}' is not a number");

static Result<int, string> ValidateAge(int age) =>
    age is >= 0 and <= 130
        ? Result.Ok<int, string>(age)
        : Result.Fail<int, string>($"age {age} out of range");

record Registration(string Email, int Age);

using System.Text.Json;
using static System.Console;

#region Constant Pattern
WriteHeaderLine("Constant Pattern");

static void ConstantPattern()
{
    object something = 42;

    // The following statement would not work (you cannot use == with
    // object and int). We need to use `Equals` instead.
    // if (something == 42) ...
    if (something.Equals(42)) WriteLine("Something is 42");

    // Now we can write this much nicer:
    //            +--- Constant pattern
    //            V
    if (something is 42) WriteLine("Something is 42");
}
ConstantPattern();
#endregion

#region Type pattern
WriteHeaderLine("Type Pattern");

// In the past we would have written...
static void GoodOldTypeCheck()
{
    // Note data type `object` in the following line
    object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
    var h = o as Hero;
    if (h != null) WriteLine($"o is a Hero and is called {h.HeroName}");
}
GoodOldTypeCheck();

// Now we can be much more concise using a "Type Pattern":
static void NewTypePattern()
{
    // Note data type `object` in the following line
    object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);

    //    +--- Type pattern
    //    |       +--- Definite assignment
    //    V       V
    if (o is Hero h)
    {
        WriteLine($"h is of type {h.GetType().Name}");
        WriteLine($"o is a Hero and is called {h.HeroName}");
    }

    // var h = 42; // does not work because h is already defined
    // WriteLine(h.HeroName); // does not work because h is unassigned outside of `if` block.

    // Avoid the following line because it is confusing
    if (!(o is Hero h2)) WriteLine("No hero");
    else WriteLine($"We have a hero named {h2.HeroName}"); // <-- because of `not` h2 is defined in `else`

    // Prefer `is not` if you want to negate condition
    if (o is not Hero h3) WriteLine("No hero");
}
NewTypePattern();

// Also works nice with collections
static void TypePatternAndCollections()
{
    // Note that Person is the base class of Hero
    IEnumerable<Person> pEnumerable = new Person[]
    {
        new("John", "Doe"),
        new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false)
    };

    //  +-- Two Type Patterns + definite assignments -+
    //  V                                             V
    if (pEnumerable is IReadOnlyList<Person> pList && pList[1] is Hero h)
    {
        WriteLine($"o is a Hero and is called {h.HeroName}");
    }
}
TypePatternAndCollections();

static void NullCheckWithTypePattern()
{
    Hero? someone = null;
    //  +-- Type Pattern     +-- h can be used in subsequent parts of expression
    //  V                    V
    if (someone is Hero h && h.Type == HeroType.FailedExperiment)
    {
        WriteLine($"Someone is the {h.HeroName} hero and not null");
    }
    else WriteLine("Someone is null");

    //  +-- Constant pattern with null
    //  V
    if (someone is null) WriteLine("Someone is null");
}
NullCheckWithTypePattern();
#endregion

#region Type pattern in Switch
WriteHeaderLine("Type Patterns in `Switch` statement");

static void TypePatternInSwitch()
{
    // Note data type `object` in the following line
    object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
    switch (o)
    {
        case "FooBar":  // <-- Constant Pattern
            WriteLine("It is a string and it is FooBar");
            break;
        case string s:  // <-- Type Pattern
            WriteLine($"o is a string and contains {s}");
            break;
        case Hero h:    // <-- Type Pattern
            WriteLine($"o is a Hero and is called {h.HeroName}");
            break;
        case Person p:  // <-- Type Pattern
            WriteLine($"o is a Person and has name {p.FirstName} {p.LastName}");
            break;
        // Try moving this case above the `Hero` case -> will result in an error
        // because the `Hero` case is no longer reachable (every `Hero` is also a `Person`).
        case var obj:   // <-- Var Pattern, catches everything
            WriteLine($"o is just an object of type {obj.GetType().Name}");
            break;
        default:
            throw new InvalidOperationException("Hmm, this should never happen...");
    }
}
TypePatternInSwitch();

static void SwitchWithWhen()
{
    // Note data type `object` in the following line
    object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
    switch (o)
    {
        case Hero h when h.Type == HeroType.FailedExperiment:
            WriteLine($"o Hero {h.HeroName} and became it because of an failed experiment");
            break;
        case Hero h:
            WriteLine($"o is Hero {h.HeroName}, NOT because of a failed experiment");
            break;
        default:
            throw new InvalidOperationException("Hmm, this should never happen...");
    }

    var type = "Fish";
    switch (type)
    {
        case "Person":
            WriteLine("We have a Person");
            break;
        case "Hero":
            WriteLine("We have a Hero");
            break;
        //   +-- Var Pattern combined with `when` makes sometimes sense
        //   V
        case var t when t.Trim().Length > 0:
            WriteLine($"We have the special type {t}");
            break;
        default:
            throw new InvalidOperationException("type must not be empty");
    }
}
SwitchWithWhen();

static void SwitchExpression()
{
    // Note data type `object` in the following line
    object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
    WriteLine(o switch
    {
        //               +--- Var pattern
        //               |      +--- Property pattern
        //               V      V
        Hero { HeroName: var n, CanFly: true } => $"Hero {n} that can fly",

        //   +--- Type pattern
        //   V
        Hero h => $"Hero {h.HeroName} {h.CanFly}",
        Person p => $"Person {p.LastName}",
        //   +--- Note throw expression
        //   V
        _ => throw new InvalidOperationException("Who is that?!?")
    });

    // Switch expression with when
    WriteLine(o switch
    {
        Hero h2 when h2.CanFly => 2,
        Hero => 1,
        Person => 0,
        _ => throw new InvalidOperationException()
    });
}
SwitchExpression();

static void SwitchWithTuples()
{
    var people = new[]
    {
        (Name: "Stan Edgar", Type: VoughtEmployeeType.TopManagement, CanFly: false, Popularity: 0),
        (Name: "Homelander", Type: VoughtEmployeeType.TheSeven, CanFly: true, Popularity: 10),
        (Name: "The Deep", Type: VoughtEmployeeType.LocalHero, CanFly: false, Popularity: 2)
    };
    var yearlySalary = people.Select(p => p switch
    {
        //             +--- Constant pattern
        //             |                                 +--- Discard pattern
        //             V                                 V
        ("Stan Edgar", VoughtEmployeeType.TopManagement, _, _) => 900_000_000,
        (_, VoughtEmployeeType.TopManagement, _, _) => 10_000_000,
        //                                                +--- Relational pattern (more later)
        //                                                V
        ("Homelander", VoughtEmployeeType.TheSeven, true, >= 9) => 100_000_000,
        ("Homelander", VoughtEmployeeType.TheSeven, true, _) => 50_000_000,
        (_, VoughtEmployeeType.TheSeven, true, _) => 25_000_000,
        (_, VoughtEmployeeType.TheSeven, _, _) => 10_000_000,
        (_, VoughtEmployeeType.LocalHero, _, _) => 1_000_000,
        _ => 25_000
    });
    WriteLine(yearlySalary.Sum());

    var peopleRecords = new Hero[]
    {
                new("Carl", "Lucas", "Luke Cage", HeroType.FailedExperiment, false),
                new("Danny", "Rand", "Iron Fist", HeroType.Other, false)
    };
    var yearlySalaryFromRecords = peopleRecords.Select(p => p switch
    {
        { CanFly: true, Type: _ } => 100_000_000, // You could omit the discard operator here
        { CanFly: _, Type: HeroType.FailedExperiment } => 50_000_000,
        _ => 1_000_000
    });
    WriteLine(yearlySalaryFromRecords.Sum());
}
SwitchWithTuples();
#endregion

#region Recursive Pattern
WriteHeaderLine("Recursive Pattern");

static void BasicRecursivePattern()
{
    Person someone = new Hero("Tony", "Stark", "Iron Man", HeroType.Technology, true);

    // Good old style:
    //  +-- Pattern matching
    //  V
    if (someone is Hero hero && hero.CanFly) WriteLine($"We have a flying hero {hero.HeroName}");

    // Recursive Pattern
    //  +-- Type Pattern
    //  |                   +-- Constant Pattern
    //  |                   |               +-- Var Pattern
    //  V                   V               V
    if (someone is Hero { CanFly: true, HeroName: var name })
    {
        WriteLine($"We have a flying hero {name}");
    }

    object somebody = new Person("Foo", "Bar", 42);
    //  +-- Type Pattern
    //  |                         +-- Relational Pattern (more will follow later)
    //  |                         |               
    //  V                         V               
    if (somebody is Person { Age: > 40 }) WriteLine("We have an old person.");

    //              +-- Empty Recursive Pattern (doesn't make sense here)
    //              V
    if (somebody is { }) WriteLine("Somebody is something");
    // Makes sense in `switch` expression
    WriteLine(somebody switch
    {
        Hero h => $"Hero {h.HeroName}",
        Person pers => $"Person {pers.FirstName}",
        { } => "Sombody not null",
        null => throw new InvalidOperationException()
    });
}
BasicRecursivePattern();

static void DoubleRecursivePattern()
{
    object bm = new Hero("Bruce", "Wayne", "Batman", HeroType.Technology, false,
        new Hero("Robin", string.Empty, "Robin", HeroType.Technology, false));
    //  +-- Type Pattern
    //  |                       +-- Type Pattern
    //  |                       |                +-- Var Pattern
    //  V                       V                V
    if (bm is Hero { Assistant: Hero { HeroName: var aName } })
    {
        WriteLine($"We have a hero who has a hero assistant named {aName}");
    }

    var o = new Hero("Bruce", "Wayne", "Batman", HeroType.Technology, false,
            new Person("Alfred", "Pennyworth"));

    // Before C# 10:
    //                 +--- Nested property syntax
    //                 V
    // if (o is Hero { Assistant: { FirstName: "Alfred "} })...

    // Since C# 10:
    //              +--- Extended property syntax
    //              V
    if (o is Hero { Assistant.FirstName: "Alfred", HeroName: var hn })
    {
        WriteLine($"{hn}'s assistant is called Alfred");
    }
}
DoubleRecursivePattern();

static void AssignmentAndBoolExpression()
{
    Person someone = new Hero("Tony", "Stark", "Iron Man", HeroType.Technology, true);

    var isHeroBecauseOfTech = someone is Hero { Type: HeroType.Technology };
    if (isHeroBecauseOfTech) WriteLine("Hero because of tech");

    if (someone is Hero { Type: HeroType.Technology }) WriteLine("Hero because of tech");
}
AssignmentAndBoolExpression();

static void RecursivePatternInSwitch()
{
    var h = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
    var ht = h switch
    {
        { Type: HeroType.NuclearAccident } => HeroTypeCategory.Accident,
        { Type: HeroType.FailedExperiment } => HeroTypeCategory.Accident,
        { Type: HeroType.Alien } => HeroTypeCategory.SuperPowersFromBirth,
        { Type: HeroType.Mutant } => HeroTypeCategory.SuperPowersFromBirth,
        _ => HeroTypeCategory.Other
    };
    WriteLine($"Hero of type {ht}");
}
RecursivePatternInSwitch();
#endregion

#region Patterns and Tuples
WriteHeaderLine("Patterns and Tuples");

static void PatternsAndTuples()
{
    var h = (HeroName: "Stormfront", CanFly: true);
    //        +-- Discard operator
    //        |  +-- Constant Pattern
    //        V  V
    if (h is (_, true)) WriteLine("The hero can fly");

    var p = (Name: "Foo Bar", Age: 42);
    //            +-- Var Pattern
    //            |  +-- Relational Pattern
    //            V  V
    if (p is (var n, > 40)) WriteLine($"Person with name {n} is not young anymore");
}
PatternsAndTuples();
#endregion

#region Relational Patterns
WriteHeaderLine("Relational Patterns");

static void SimpleRelational()
{
    var age = 42;
    //  +-- Relational Pattern
    //  V
    if (age is >= 42) WriteLine("High");

    int? unknownAge = null;
    //                +-- Negation
    //                V
    if (unknownAge is not >= 42) WriteLine("Low or null");
}
SimpleRelational();

static void RelationalCombination()
{
    var letter = 'x';
    //                                           +- Combination and/or -+
    //                                           V           V          V
    static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
    WriteLine(IsLetter(letter));

    var number = 42;
    //                                        +-- Combinator
    //                                        V
    static bool IsInRange(int n) => n is > 40 and < 50;
    WriteLine(IsInRange(number));

    object someObject = 420;
    //             +-- Type Pattern...
    //             |      +-- ...combined with relational pattern
    //             V      V
    if (someObject is int and not < 42) WriteLine("High Number");
}
RelationalCombination();

static void RelationalInSwitch()
{
    var p = new Person("Foo", "Bar", 13);
    var ag = p.Age switch //  <-------------+
    {//                                     |
     //  +-- Relational Pattern in switch --+
     //  V
        < 13 => "child",
        < 18 => "teenager",
        < 65 => "adult",
        _ => "senior"
    };
    WriteLine($"Person is in age group {ag}");
}
RelationalInSwitch();
#endregion

#region List patterns
WriteHeaderLine("List Patterns");

static void ListPatterns()
{
    var numbers = new [] { 1, 2, 3, 5, 8 };

    //          +--- List pattern
    //          V
    if (numbers is [1, 2, 3, 5, 8])
    {
        WriteLine("Fibonacci");
    }

    //              +--- Note var pattern
    //              V
    if (numbers is [var first, 2, 3, 5, var last] && first == 1 && last == 8)
    {
        WriteLine("Very special Fibonacci");
    }

    if (numbers is [var one, .. var someNumbers, 8])
    {
        WriteLine(JsonSerializer.Serialize(someNumbers));
    }

    // Slice pattern
    WriteLine(numbers switch
    {
        //  +--- Slice pattern
        //  V
        [1, .., var sl, 8] => $"Starts with 1, ends with 8, and 2nd last number is {sl}",

        //              +--- Relational pattern inside list pattern
        //              V
        [1, .., var sl, > 8 or < 0] =>
            $"Starts with 1, ends with something > 8 or < 0, and 2nd last number is {sl}",

        //  +--- Discard pattern
        //  V
        [1, _, _, ..] => "Starts with 1 and is at least 3 long",
        [1, .. var rest] => $"Starts with 1 and is followed by {rest.Length} numbers",
        _ => "WAT?"
    });
}
ListPatterns();

static void Something()
{
    Span<int> numbers = new int[] { 1, 2, 3, 4 };

    WriteLine(numbers switch
    {
        [1, .. var middle, var x, 5] => middle.Length,
        _ => "asdf"
    });

}

static void ListPatternsCombined()
{
    var heroes = new List<JumpingHero>
    {
        new("Superman", int.MaxValue),
        new("The Tick", 10),
    };

    //               +--- Property pattern inside list pattern
    //               |                +--- Relational pattern
    //               V                V
    if (heroes is [{ MaxJumpDistance: > 1000 }, { MaxJumpDistance: < 100, Name: var snd }])
    {
        WriteLine($"First can fly, second ('{snd}') cannot jump very far");
    }
}
ListPatternsCombined();
#endregion

#region Span Matching
ReadOnlySpan<char> span = "Homelander";

// Pre-C# 11
switch (span)
{
    case var _ when span == "Starlight":
        WriteLine("We have Starlight");
        break;
    case var _ when span == "The Deep":
        WriteLine("We have The Deep");
        break;
    case var _ when span == "Homelander":
        WriteLine("We have Homelander");
        break;
    default:
        WriteLine("We have someone else");
        break;
}

// Now
switch (span)
{
    case "Starlight":
        WriteLine("We have Starlight");
        break;
    case "The Deep":
        WriteLine("We have The Deep");
        break;
    case "Homelander":
        WriteLine("We have Homelander");
        break;
    default:
        WriteLine("We have someone else");
        break;
}

if (span is "Homelander")
{
    WriteLine("We have Homelander");
}
#endregion

#region Helper methods and data structures
static void WriteHeaderLine(string message)
{
    var oldFo = ForegroundColor;
    ForegroundColor = ConsoleColor.Yellow;
    WriteLine($"\n{message}");
    ForegroundColor = oldFo;
}

enum HeroType { NuclearAccident, FailedExperiment, Alien, Mutant, Technology, Other };

enum HeroTypeCategory { Accident, SuperPowersFromBirth, Other }

enum VoughtEmployeeType { TopManagement, TheSeven, LocalHero, RegularPerson };

record Person(string FirstName, string LastName, int? Age = null, Person? Assistant = null);

record Hero(string FirstName, string LastName, string HeroName, HeroType Type,
    bool CanFly, Person? Assistant = null) : Person(FirstName, LastName, Assistant: Assistant);

record JumpingHero(string Name, int MaxJumpDistance);
#endregion

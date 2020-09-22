using System;
using System.Collections.Generic;
using static System.Console;

static class Program
{
    enum HeroType { NuclearAccident, FailedExperiment, Alien, Mutant, Technology, Other };

    enum HeroTypeCategory { Accident, SuperPowersFromBirth, Other }

    record Person(string FirstName, string LastName, int? Age = null, Person? Assistant = null);

    record Hero(string FirstName, string LastName, string HeroName, HeroType Type, bool CanFly, Person? Assistant = null)
        : Person(FirstName, LastName, Assistant: Assistant);

    public static void Main()
    {
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
            if (something is 42) WriteLine("Something is 42");
        }
        ConstantPattern();
        #endregion

        #region Type pattern
        WriteHeaderLine("Type Pattern");

        // In the past we would have written...
        static void GoodOldTypeCheck()
        {
            object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
            var h = o as Hero;
            if (h != null) WriteLine($"o is a Hero and is called {h.HeroName}");
        }
        GoodOldTypeCheck();

        // Now we can be much more concise using a "Type Pattern":
        static void NewTypePattern()
        {
            object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
            if (o is Hero h) // <- Type Pattern
            {
                WriteLine($"h is of type {h.GetType().Name}");
                WriteLine($"o is a Hero and is called {h.HeroName}");
            }

            // var h = 42; // does not work because h is already defined
            // WriteLine(h.HeroName); // does not work because h is unassigned outside of `if` block

            // Avoid the following line because it is confusing
            if (!(o is Hero h2)) WriteLine("No hero");
            else WriteLine($"We have a hero named {h2.HeroName}"); // <-- because of `not` h2 is defined in `else`
        }
        NewTypePattern();

        // Also works nice with collections
        static void TypePatternAndCollections()
        {
            IEnumerable<Person> pEnumerable = new Person[]
            {
                new("John", "Doe"),
                new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false)
            };

            //  +-- Two Type Patterns ------------------------+
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
            object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
            switch (o)
            {
                case "FooBar": // <-- Constant Pattern
                    WriteLine("It is a string and it is FooBar");
                    break;
                case string s: // <-- Type Pattern
                    WriteLine($"o is a string and contains {s}");
                    break;
                case Hero h: // <-- Type Pattern
                    WriteLine($"o is a Hero and is called {h.HeroName}");
                    break;
                case Person p: // <-- Type Pattern
                    WriteLine($"o is a Person and has name {p.FirstName} {p.LastName}");
                    break;
                // Try moving this case above the `Hero` case -> will result in an error
                // because the `Hero` case is no longer reachable (every `Hero` is also a `Person`).
                case var obj: // <-- Var Pattern, catches everything
                    WriteLine($"o is just an object of type {obj.GetType().Name}");
                    break;
                default:
                    throw new InvalidOperationException("Hmm, this should never happen...");
            }
        }
        TypePatternInSwitch();

        static void SwitchWithWhen()
        {
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
            object o = new Hero("Wade", "Wilson", "Deadpool", HeroType.FailedExperiment, false);
            WriteLine(o switch
            {
                Hero { HeroName: var n, CanFly: true } => $"Hero {n} that can fly",
                Hero h => $"Hero {h.HeroName} {h.CanFly}",
                Person p => $"Person {p.LastName}",
                _ => "Who is that?!?"
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
            WriteLine(("Foo", "Bar", "General", 42) switch
            {
                //  +-- Discard operator
                //  |             +-- Relational Operator
                //  V             V
                (_, _, "General", > 60) => 99,
                (_, _, "General", _) => 90,
                (_, _, _, > 60) => 80,
                ("Foo", _, _, _) => 70,
                _ => 60
            });
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
            if (somebody is Person { Age: > 40 } p) WriteLine("We have an old person.");

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
            //            +-- Constant Pattern
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
            var ag = p.Age switch // <-------------+
            {//                                    |
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
    }

    private static void WriteHeaderLine(string message)
    {
        var (oldBg, oldFo) = (BackgroundColor, ForegroundColor);
        (BackgroundColor, ForegroundColor) = (ConsoleColor.Red, ConsoleColor.White);
        WriteLine(message);
        (BackgroundColor, ForegroundColor) = (oldBg, oldFo);
    }
}

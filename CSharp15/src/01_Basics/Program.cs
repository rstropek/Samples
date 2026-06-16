// ============================================================================
//  C# 15 — Union types: the basics
// ----------------------------------------------------------------------------
//  A `union` declares a value that is exactly one of a closed set of case types.
//  The compiler:
//    * provides an *implicit conversion* from each case type into the union,
//    * makes `switch` *exhaustive* (no `default` arm required),
//    * "unwraps" the value automatically when pattern matching.
// ============================================================================

namespace Basics;

// The case types. Any type works (records keep the demo terse).
public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name, bool CanFly);

// The union declaration. This single line is the headline C# 15 feature.
public union Pet(Cat, Dog, Bird);

internal static class Program
{
    private static void Main()
    {
        // --- 1. Implicit conversion: a Dog *is* a Pet, no wrapping ceremony ---
        Pet a = new Dog("Rex");          // implicit case -> union conversion
        Pet b = new Cat("Whiskers");
        Pet c = new Bird("Tweety", CanFly: true);

        // --- 2. Exhaustive switch: every case handled, NO default needed ------
        Console.WriteLine(Describe(a));
        Console.WriteLine(Describe(b));
        Console.WriteLine(Describe(c));

        // --- 3. Unions are ordinary values: store them in collections --------
        Pet[] shelter = [a, b, c, new Dog("Buddy")];
        int dogs = shelter.Count(p => p is Dog);
        Console.WriteLine($"\nShelter holds {shelter.Length} pets, {dogs} of them dogs.");

        // --- 4. Pattern matching "reaches through" the union -----------------
        //     Property patterns apply to the *unwrapped* case value.
        foreach (var pet in shelter)
        {
            if (pet is Bird { CanFly: true } bird)
                Console.WriteLine($"{bird.Name} can fly away!");
        }

        // --- 5. The escape hatch: every union exposes `object? Value` --------
        object? raw = a.Value;           // the boxed/stored case instance
        Console.WriteLine($"\nUnderlying runtime type of `a`: {raw?.GetType().Name}");

        // --- 6. For contrast: how this was modelled BEFORE C# 15 (see docs §1) -
        Alternatives.AllDemos.Run();
    }

    // Exhaustive over { Cat, Dog, Bird }. Add a 4th case type to the union and
    // this method stops compiling cleanly (CS8509) until you handle it.
    private static string Describe(Pet pet) => pet switch
    {
        Cat c           => $"Cat named {c.Name}",
        Dog d           => $"Dog named {d.Name}",
        Bird { CanFly: true } b  => $"Bird named {b.Name} (flies)",
        Bird b          => $"Bird named {b.Name} (grounded)",
        null            => "<uninitialised pet>",   // the default/empty union (CS8655)
    };
}

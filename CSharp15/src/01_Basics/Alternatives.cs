// ============================================================================
//  BEFORE C# 15 — how the same "a pet is one of Cat / Dog / Bird" problem was
//  modelled, and what each approach cost (see docs/01-CSharp15-Unions.md §1).
//
//  Each approach lives in its own namespace so the domain type names don't
//  collide with the `union Pet(Cat, Dog, Bird)` in Program.cs.
// ============================================================================

namespace Basics.Alternatives
{
    public static class AllDemos
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Pre-C#15 alternatives (same Cat/Dog/Bird problem) ===");
            ClassHierarchy.Demo.Run();
            ObjectAndIs.Demo.Run();
            NullableFields.Demo.Run();
            OneOfLibrary.Demo.Run();
        }
    }
}

// ---------------------------------------------------------------------------
// (1) Abstract base + sealed subclasses (a "closed-ish" hierarchy).
//     Cost: the compiler does NOT treat a sealed hierarchy as exhaustive yet,
//     so the switch still needs a `_` / default arm or it warns (CS8509).
// ---------------------------------------------------------------------------
namespace Basics.Alternatives.ClassHierarchy
{
    public abstract record Pet;
    public sealed record Cat(string Name) : Pet;
    public sealed record Dog(string Name) : Pet;
    public sealed record Bird(string Name, bool CanFly) : Pet;

    public static class Demo
    {
        public static string Describe(Pet pet) => pet switch
        {
            Cat c  => $"Cat named {c.Name}",
            Dog d  => $"Dog named {d.Name}",
            Bird b => $"Bird named {b.Name}",
            // Required: nothing stops a future subclass, so the compiler can't
            // prove exhaustiveness. With a union this arm disappears.
            _      => throw new InvalidOperationException("unhandled Pet"),
        };

        public static void Run()
        {
            Pet p = new Dog("Rex");
            Console.WriteLine($"1) class hierarchy   -> {Describe(p)} (needs a default arm)");
        }
    }
}

// ---------------------------------------------------------------------------
// (2) `object` + `is` checks.
//     Cost: no type safety and no exhaustiveness — `string` compiles fine and
//     fails only at runtime; forgetting a case is silent.
// ---------------------------------------------------------------------------
namespace Basics.Alternatives.ObjectAndIs
{
    public record Cat(string Name);
    public record Dog(string Name);
    public record Bird(string Name, bool CanFly);

    public static class Demo
    {
        public static string Describe(object pet)
        {
            if (pet is Cat c)  return $"Cat named {c.Name}";
            if (pet is Dog d)  return $"Dog named {d.Name}";
            if (pet is Bird b) return $"Bird named {b.Name}";
            return "<unknown — anything can be passed here>";   // even `object pet = "oops"`
        }

        public static void Run()
        {
            object p = new Cat("Whiskers");
            Console.WriteLine($"2) object + is       -> {Describe(p)} (no type safety)");
        }
    }
}

// ---------------------------------------------------------------------------
// (3) One class with a nullable field per case.
//     Cost: illegal states are representable — you can set two at once, or
//     none, and the invariant "exactly one" lives only in comments.
// ---------------------------------------------------------------------------
namespace Basics.Alternatives.NullableFields
{
    public record Cat(string Name);
    public record Dog(string Name);
    public record Bird(string Name, bool CanFly);

    public sealed class Pet   // invariant (uncheckable): exactly one field is non-null
    {
        public Cat? Cat { get; init; }
        public Dog? Dog { get; init; }
        public Bird? Bird { get; init; }
    }

    public static class Demo
    {
        public static string Describe(Pet pet) =>
            pet.Cat is { } c  ? $"Cat named {c.Name}" :
            pet.Dog is { } d  ? $"Dog named {d.Name}" :
            pet.Bird is { } b ? $"Bird named {b.Name}" :
            "<no case set>";

        public static void Run()
        {
            var p = new Pet { Dog = new Dog("Buddy") };
            // Nothing stops this illegal value: var bad = new Pet { Cat = ..., Dog = ... };
            Console.WriteLine($"3) nullable fields   -> {Describe(p)} (illegal states representable)");
        }
    }
}

// ---------------------------------------------------------------------------
// (4) The OneOf NuGet package.
//     Cost: positional API (.Match arms by position, .IsT0/.AsT0), generic
//     noise, and a third-party dependency — but it does give a real "one of".
// ---------------------------------------------------------------------------
namespace Basics.Alternatives.OneOfLibrary
{
    using OneOf;

    public record Cat(string Name);
    public record Dog(string Name);
    public record Bird(string Name, bool CanFly);

    public static class Demo
    {
        public static string Describe(OneOf<Cat, Dog, Bird> pet) =>
            pet.Match(                          // arms are positional, not named
                cat  => $"Cat named {cat.Name}",
                dog  => $"Dog named {dog.Name}",
                bird => $"Bird named {bird.Name}");

        public static void Run()
        {
            OneOf<Cat, Dog, Bird> p = new Bird("Tweety", true);  // implicit conversion
            Console.WriteLine($"4) OneOf<...>        -> {Describe(p)}  (IsT2={p.IsT2})");
        }
    }
}

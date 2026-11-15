// ============================================================================
//  C# 15 — Closed hierarchies: the OTHER half of C# 15's exhaustiveness story.
// ----------------------------------------------------------------------------
//  `closed` on a class/record means: only types in THIS assembly may derive
//  from it directly. The set of direct subtypes is fixed at compile time, so a
//  switch over the base type is exhaustive without a `_` / default arm.
//
//  Compare with 01_Basics/Alternatives.cs (1): the abstract + sealed hierarchy
//  needed a throwing `_` arm. `closed` removes it.
//
//  Union vs closed — rule of thumb:
//    * union  : "one of these EXISTING types" (int, string, someone else's
//               record, ...). No shared base, wraps the value in a struct.
//    * closed : "one of MY subtypes". You own the cases, they share a base
//               (and its members), no wrapper, no boxing, no default-empty
//               value — but `null` is still a possible reference.
// ============================================================================
namespace ClosedHierarchies;

// --- (1) The Pet example again, as a closed hierarchy -----------------------
// A closed class is implicitly abstract (combining it with `abstract`, `sealed`
// or `static` is an error). Shared data lives on the base: `Name` is available
// on every Pet without a switch — something a union of unrelated types can't do.
public closed record Pet(string Name);
public record Cat(string Name) : Pet(Name);
public record Dog(string Name) : Pet(Name);
public record Bird(string Name, bool CanFly) : Pet(Name);

// Closed-ness is NOT transitive: Dog is an ordinary (open) record, so other
// assemblies may derive from Dog. That does not break exhaustiveness over Pet,
// because a Puppy is still matched by the `Dog` arm.
public record Puppy(string Name) : Dog(Name);

// In ANOTHER assembly this fails to compile:
//   public record Hamster(string Name) : Pet(Name);
//   error CS9382: 'Hamster': cannot use a closed type 'Pet' from another assembly as a base type.

// --- (2) A generic, recursive closed hierarchy ------------------------------
// Rule for generics: every type parameter of a direct subtype must appear in
// the base-class specification (`Leaf<T> : Tree<T>` OK, `Const<U> : Tree<int>` not).
public closed record Tree<T>;
public record Leaf<T>(T Value) : Tree<T>;
public record Node<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;

internal static class Program
{
    // Exhaustive over { Cat, Dog, Bird } — no `_` arm, no warning.
    // Remove an arm and you get CS8509 ("the pattern 'Bird' is not covered").
    static string Describe(Pet pet) => pet switch
    {
        Cat c                  => $"Cat named {c.Name}",
        Dog d                  => $"Dog named {d.Name}",   // also matches Puppy
        Bird { CanFly: true } b => $"Bird named {b.Name} (flies)",
        Bird b                 => $"Bird named {b.Name} (grounded)",
    };

    // For a NULLABLE governing type, `null` must be handled as well —
    // omitting this arm gives CS8655 ("the pattern 'null' is not covered").
    static string DescribeMaybe(Pet? pet) => pet switch
    {
        null  => "<no pet>",
        Pet p => Describe(p),
    };

    // A type parameter constrained to a closed class is treated like the class
    // itself for exhaustiveness.
    static string Sound<TPet>(TPet pet) where TPet : Pet => pet switch
    {
        Cat  => "meow",
        Dog  => "woof",
        Bird => "tweet",
    };

    // Folds over a closed recursive hierarchy — the AST style of 03_RecursiveExpr,
    // without a union wrapper.
    static int Sum(Tree<int> t) => t switch
    {
        Leaf<int>(var v)      => v,
        Node<int>(var l, var r) => Sum(l) + Sum(r),
    };

    static int Depth<T>(Tree<T> t) => t switch
    {
        Leaf<T>             => 1,
        Node<T>(var l, var r) => 1 + Math.Max(Depth(l), Depth(r)),
    };

    private static void Main()
    {
        Pet[] pets = [new Cat("Whiskers"), new Dog("Rex"), new Puppy("Bit"), new Bird("Tweety", CanFly: true)];

        foreach (var pet in pets)
        {
            // Shared base member — no pattern matching needed for `Name`.
            Console.WriteLine($"{pet.Name,-9} {Describe(pet),-28} says {Sound(pet)}");
        }
        Console.WriteLine(DescribeMaybe(null));

        // Unlike a union, there's no wrapper: a Pet IS the case object.
        Pet rex = pets[1];
        Console.WriteLine($"\nRuntime type of a Pet: {rex.GetType().Name} (no wrapper struct, no .Value)");

        Tree<int> tree = new Node<int>(
            new Leaf<int>(1),
            new Node<int>(new Leaf<int>(2), new Leaf<int>(3)));
        Console.WriteLine($"\nTree sum = {Sum(tree)}, depth = {Depth(tree)}");
    }
}

// ============================================================================
//  BONUS C# 15 feature — Collection expression arguments.
//  A `with(...)` element (must be FIRST) forwards arguments to the target
//  collection's constructor / factory: capacity, comparers, etc.
// ============================================================================
namespace CollectionArgs;

internal static class Program
{
    private static void Main()
    {
        string[] values = ["one", "two", "three"];

        // 1. Pre-size a List<T> by passing `capacity` to its constructor.
        List<string> names = [with(capacity: values.Length * 2), .. values];
        Console.WriteLine($"List: count={names.Count}, capacity={names.Capacity}");

        // 2. Pass an IEqualityComparer to a HashSet<T> constructor.
        HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
        Console.WriteLine($"HashSet (OrdinalIgnoreCase): count={set.Count}"); // 1

        // 3. Pass a custom IComparer to a SortedSet constructor (reverse order).
        SortedSet<int> descending =
            [with(Comparer<int>.Create((x, y) => y.CompareTo(x))), 3, 1, 2];
        Console.WriteLine($"SortedSet (reverse): [{string.Join(", ", descending)}]"); // 3, 2, 1

        // 4. `with(...)` composes with spreads and explicit elements together.
        List<int> merged = [with(capacity: 16), 0, .. Enumerable.Range(1, 3), 4];
        Console.WriteLine($"Merged: [{string.Join(", ", merged)}] capacity={merged.Capacity}");
    }
}

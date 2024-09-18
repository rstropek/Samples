using System;
using System.Collections.Generic;
using System.Text;

// Prior to C# 13, 'params' could only be used with arrays.
// So, we could write:
PrintNumbersArray(1, 2, 3, 4, 5);

// But we could not use 'params' with other collection types like List<T> or Span<T>.
// The following lines would cause compilation errors in previous C# versions:
// PrintNumbersList(1, 2, 3, 4, 5);
// PrintNumbersSpan(1, 2, 3, 4, 5);

// With C# 13's 'params collections' feature, we can now use 'params' with List<int>:
PrintNumbersList(1, 2, 3, 4, 5);

// Similarly, we can use 'params' with IEnumerable<int>:
PrintNumbersEnumerable(1, 2, 3, 4, 5);

// And we can use 'params' with Span<int> for performance benefits:
PrintNumbersSpan(1, 2, 3, 4, 5);

// We can also use 'params' with ReadOnlySpan<int>:
PrintNumbersReadOnlySpan(1, 2, 3, 4, 5);
// This is particularly interesting for performance reasons. Look at the generated IL code
// (e.g. with dnSpyEx) to see how it works.

// Using 'params' with an array (possible in all C# versions)
static void PrintNumbersArray(params int[] numbers)
{
    Console.WriteLine("PrintNumbersArray:");
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
    Console.WriteLine();
}

// Using 'params' with List<int> (new in C# 13)
// This was not possible in previous C# versions
static void PrintNumbersList(params List<int> numbers)
{
    Console.WriteLine("PrintNumbersList (List<int>):");
    // The 'numbers' parameter is a single List<int> constructed from the arguments
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
    Console.WriteLine();
}

// Using 'params' with IEnumerable<int> (new in C# 13)
// This allows flexibility in the type of collection used
static void PrintNumbersEnumerable(params IEnumerable<int> numbers)
{
    Console.WriteLine("PrintNumbersEnumerable (IEnumerable<int>):");
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
    Console.WriteLine();
}

// Using 'params' with Span<int> (new in C# 13)
// Offers performance benefits by avoiding heap allocations
static void PrintNumbersSpan(params Span<int> numbers)
{
    Console.WriteLine("PrintNumbersSpan (Span<int>):");
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
    Console.WriteLine();
}

// Using 'params' with ReadOnlySpan<int> (new in C# 13)
// Useful for read-only operations with performance gains
static void PrintNumbersReadOnlySpan(params ReadOnlySpan<int> numbers)
{
    Console.WriteLine("PrintNumbersReadOnlySpan (ReadOnlySpan<int>):");
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
    Console.WriteLine();
}


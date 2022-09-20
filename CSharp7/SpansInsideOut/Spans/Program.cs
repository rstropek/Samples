using Spans;
using System.Text.Json;

// Sort some numbers in-place
var numbers = new[] { 5, 4, 3, 2, 1 };
Sorter.BubbleSort(numbers);
Printer.Print(numbers);

// Ignore the last two numbers when sorting the array.
// This solution produces a log of garbage!
numbers = new[] { 5, 4, 3, 2, 1 };
var numbersToSort = numbers[..^2];
Sorter.BubbleSort(numbersToSort);
numbersToSort.CopyTo(numbers, 0);
Printer.Print(numbers);

// Let's try the same with our span
MySpan<int> numbersMySpan = new[] { 5, 4, 3, 2, 1 };
Sorter.BubbleSort(numbersMySpan[..^2]);
Printer.Print(numbersMySpan);

// C#'s Span<T> can not only handle array on the heap, but also
// arrays on the stack and in unmanaged memory.
Span<int> numbersSpan = new[] { 5, 4, 3, 2, 1 };

//Span<int> numbersSpan = stackalloc int[] { 5, 4, 3, 2, 1 };

//numbers = new[] { 5, 4, 3, 2, 1 };
//Span<int> numbersSpan;
//IntPtr ptr = Marshal.AllocHGlobal(numbers.Length * sizeof(int));
//try
//{
//    unsafe { numbersSpan = new Span<int>((int*)ptr, numbers.Length); }
//    numbers.CopyTo(numbersSpan);
//}
//finally
//{
//    Marshal.FreeHGlobal(ptr);
//}

Sorter.BubbleSort(numbersSpan[..^2]);
Printer.Print(numbersSpan);

// We can iterate over elements of a span
foreach (var number in numbersSpan)
{
    Console.WriteLine(number);
}

// But we cannot use Linq
// numbersSpan.Where(n => n % 2 == 0) -> results in an error

// Spans work with strings
ReadOnlySpan<char> letters = "20,22".AsSpan();
var number1 = Int32.Parse(letters[..2]);
var number2 = Int32.Parse(letters[^2..]);
Console.WriteLine(number1 + number2);

// But spans don't work with lists
// numbersSpan = new List<int> { 5, 4, 3, 2, 1 }; -> results in an error

// We cannot return a stackalloc'ed span
Span<int> ReturnSomeNumbers() => /*stackalloc*/ new int[] { 5, 4, 3, 2, 1 };
numbersSpan = ReturnSomeNumbers();
Sorter.BubbleSort(numbersSpan[..^2]);
Printer.Print(numbersSpan);

// Span is a ref struct -> can never leave stack
// object o = new Span<int>(new[] { 5, 4, 3, 2, 1 }); -> results in an error
Func<int> first = () =>
{
    Span<int> innerNumbersSpan = new[] { 5, 4, 3, 2, 1 };
    // return new Func<int>(() => innerNumbersSpan[0]); -> results in an error
    return 42;
};
//var listOfSpans = new List<Span<int>>(42); -> results in an error

// Strings are immutable? Well, not always. With `String.Create` you have the chance to get 
// a mutable version of the string at construction time. Note that the following sample is 
// simplified because it can only handle single-digit numbers. Your real-world algorithms will
// be more complicated.
var csvNumbers = String.Create(numbers.Length - 1 + numbers.Length, numbers, (buffer, source) =>
{
    for (var i = 0; i < source.Length; i++)
    {
        if (i > 0)
        {
            buffer[0] = ',';
            buffer = buffer[1..];
        }

        source[i].TryFormat(buffer, out var charsWritten);
        buffer = buffer[charsWritten..];
    }
});
Console.WriteLine(csvNumbers + "!");

// What should you do if the limitations of Span are a problem for your use case
// (e.g. you need to use it with lambdas, in classes, as type parameters)?
// Memory<T> is your friend in such cases.
var manyNumbers = new List<Memory<int>>
        {
            new [] { 5, 4, 3, 2, 1 },
            new [] { 9, 8, 7, 6, 5 }
        };
manyNumbers.ForEach(numbersMemory =>
{
    // Get a Span from the Memory for local processing
    var localSpan = numbersMemory.Span;
    Sorter.BubbleSort(localSpan);
    Printer.Print(localSpan);
});

#region Sort algorithms
public static class Sorter
{
    public static void BubbleSort<T>(T[] list) where T : IComparable
    {
        bool madeChanges;
        int itemCount = list.Length;
        do
        {
            madeChanges = false;
            itemCount--;
            for (int i = 0; i < itemCount; i++)
            {
                if (list[i].CompareTo(list[i + 1]) > 0)
                {
                    T temp = list[i + 1];
                    list[i + 1] = list[i];
                    list[i] = temp;
                    madeChanges = true;
                }
            }
        } while (madeChanges);
    }

    public static void BubbleSort<T>(MySpan<T> list) where T : IComparable
    {
        bool madeChanges;
        int itemCount = list.Length;
        do
        {
            madeChanges = false;
            itemCount--;
            for (int i = 0; i < itemCount; i++)
            {
                if (list[i].CompareTo(list[i + 1]) > 0)
                {
                    T temp = list[i + 1];
                    list[i + 1] = list[i];
                    list[i] = temp;
                    madeChanges = true;
                }
            }
        } while (madeChanges);
    }

    public static void BubbleSort<T>(Span<T> list) where T : IComparable
    {
        bool madeChanges;
        int itemCount = list.Length;
        do
        {
            madeChanges = false;
            itemCount--;
            for (int i = 0; i < itemCount; i++)
            {
                if (list[i].CompareTo(list[i + 1]) > 0)
                {
                    T temp = list[i + 1];
                    list[i + 1] = list[i];
                    list[i] = temp;
                    madeChanges = true;
                }
            }
        } while (madeChanges);
    }
}
#endregion

#region Helper methods
public static class Printer
{
    public static void Print<T>(T[] list) => Console.WriteLine(JsonSerializer.Serialize(list));
    public static void Print<T>(MySpan<T> list) => Printer.Print(list.ToArray());
    public static void Print<T>(Span<T> list) => Printer.Print(list.ToArray());
}
#endregion

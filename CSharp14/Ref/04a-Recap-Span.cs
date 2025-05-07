using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Ref;

public static class Span
{
    public static void TheProblem()
    {
        void BubbleSort<T>(T[] list) where T : IComparable
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
                        (list[i], list[i + 1]) = (list[i + 1], list[i]);
                        madeChanges = true;
                    }
                }
            } while (madeChanges);
        }

        void BubbleSortSpan<T>(scoped Span<T> list) where T : IComparable
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
                        (list[i], list[i + 1]) = (list[i + 1], list[i]);
                        madeChanges = true;
                    }
                }
            } while (madeChanges);
        }

        // The problem:
        // We have a list of numbers and want to sort them
        byte[] numbers = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        BubbleSort(numbers);
        Console.WriteLine(string.Join(", ", numbers));

        // Now we want to sort a part of the array -> we have to copy
        numbers = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        var someNumbers = new byte[numbers.Length - 2];
        for (var i = 1; i < numbers.Length - 1; i++) { someNumbers[i - 1] = numbers[i]; }
        BubbleSort(someNumbers);
        Console.WriteLine(string.Join(", ", someNumbers));

        // Span solves our problem
        Span<byte> numbersSpan = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        BubbleSortSpan(numbersSpan[1..^1]);
        foreach (var n in numbersSpan) { Console.Write($"{n} "); }
    }

    public static void SpanBasics()
    {
        // Create a span based on an array (heap)
        byte[] arr = [1, 2, 3, 5, 8, 13, 21, 34, 55];
        Span<byte> bytes = arr;

        // You can do the same on the stack
        // BTW: Note that stackalloc works with Span<T> even outside of unsafe context.
        Span<byte> stackBytes = stackalloc byte[] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };

        // Access element by index
        bytes[0] = 1;
        stackBytes[0] = 1;

        // Create a slice - which is a Span<t> itself - of the array
        // Note the use of a read-only span here.
        ReadOnlySpan<byte> slice = bytes.Slice(1, 2);

        // You can also use C#'s range feature
        PrintHeadline("Spans and ranges:");
        ReadOnlySpan<byte> sliceFromRange = bytes[1..^1];
        Console.WriteLine($"\tLength of source was {bytes.Length}, new slice has length {sliceFromRange.Length}");

        // Iterate over slice using a for-loop
        // BTW: Did you know that C# eliminates bounds checks in cases like this?
        //      Read more at https://blogs.msdn.microsoft.com/clrcodegeneration/2009/08/13/array-bounds-check-elimination-in-the-clr/
        PrintHeadline("Iterate using for-loop:");
        for (var i = 0; i < slice.Length; i++)
        {
            // Access an item in a span using indexer.
            // Exercise: Find the indexer in the Span<T> class and check its return type. Looks familiar 😉?
            Console.WriteLine($"\t{slice[i]}");
        }

        // Iterate over slice using foreach
        // Note that Span<T> does not implement IEnumerable but has a GetEnumerator-member.
        // Therefore, foreach works. However, Linq does not because it would require IEnumerable.
        // BTW: This works because of the concept of "Duck Typing".
        //      Read more at https://blogs.msdn.microsoft.com/kcwalina/2007/07/18/duck-notation/
        PrintHeadline("Iterate using foreach-loop:");
        foreach (var value in stackBytes)
        {
            Console.WriteLine($"\t{value}");
        }

        // Spans and strings
        PrintHeadline("Spans and strings:");
        var answer = "The answer is 42";
        // Note that the next line is brand new in C# 14. See
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/first-class-span-types
        ReadOnlySpan<char> answerSpan = answer;
        // Note that int.Parse accepts a ReadOnlySpan<char> as well
        Console.WriteLine("\t" + int.Parse(answerSpan[answerSpan.IndexOf('4')..]));

        // Strings, spans, and ranges
        ReadOnlySpan<char> text = "Temp: 15° C";
        text = text[(text.IndexOf(' ') + 1)..];
        text = text[..(text.IndexOf(' ') - 1)];
        if (int.TryParse(text, out var temperature)) 
        {
            Console.WriteLine($"\t{temperature}");
        }

        // Spans with UTF8 literal
        ReadOnlySpan<byte> utf8 = "Hello World"u8;
        Console.Write('\t');
        for (var i = 0; i < utf8.Length; i++)
        {
            // Note that the UTF8 string is a byte array
            Console.Write((char)utf8[i]);
        }
        Console.WriteLine();

        // A span can be constructed from a single value
        PrintHeadline("Single-value span:");
        int singleValue = 42;
        Span<int> singleValueSpan = new(ref singleValue);
        Console.WriteLine($"\tLength: {singleValueSpan.Length}, Value: {singleValueSpan[0]}");

        // Spans can be easily filled and cleared
        // Note the use of the new string.Join overload. Starting with .NET 9, string.Join
        // can take a ReadOnlySpan as an input.
        PrintHeadline("Clearning and filling:");
        Span<string> greetings = new string[3];
        greetings.Fill("Hello");
        Console.WriteLine($"\t{string.Join(", ", greetings)}");
        greetings.Clear();
        Console.WriteLine($"\t{greetings[0] ?? "null"}");

        // scoped parameters. We can pass the stack-allocated array directly to the method
        // because the parameter of the method is scoped.
        ReadOnlySpan<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };
        var consumer = new SpanAggregator();
        consumer.RememberSum(numbers);
        Console.WriteLine($"\tSum: {consumer.Sum}");
    }

    public static void RefConversions()
    {
        // Note: Implicit conversions now works with C# 14

        // From Span to ReadOnlySpan
        Span<int> numbers = [1, 2, 3, 4, 5];
        ReadOnlySpan<int> readOnlyNumbers = numbers;

        // From single-dimensional array to Span and ReadOnlySpan
        int[] numbersArray = [1, 2, 3, 4, 5];
        Span<int> numbersFromArray = numbersArray;
        ReadOnlySpan<int> readOnlyNumbersFromArray = numbersArray;

        // From Span<T> to ReadOnlySpan<U> where T is covariance-compatible to U
        ReadOnlySpan<SomeBase> someBases = new Span<SomeDerived>(new SomeDerived[5]);
    }

    class SomeBase { }
    class SomeDerived : SomeBase { }

    ref struct SpanAggregator
    {
        public int Sum { get; private set; }

        // public ReadOnlySpan<int> SourceNumbers { get; private set; }

        // Note scoped parameter. This is important to make sure
        // that the span is not used after the method returns.
        public int RememberSum(scoped ReadOnlySpan<int> numbers)
        {
            // The following line is not allwed because the span is scoped.
            // SourceNumbers = numbers;

            var sum = 0;
            foreach (var number in numbers) { sum += number; }
            return sum;
        }
    }

    public static void SlicesInStandardLibrary()
    {
        // New in .NET 9: File helpers supporting spans
        PrintHeadline("File helpers with spans:");
        ReadOnlySpan<char> text = stackalloc char[] { 'H', 'e', 'l', 'l', 'o', ' ', 'W', 'o', 'r', 'l', 'd' };
        File.WriteAllText("hello.txt", text);
        Console.WriteLine($"\t{File.ReadAllText("hello.txt")}");
        File.Delete("hello.txt");

        // Lots of string functions support Spans
        ReadOnlySpan<char> part1 = stackalloc char[] { 'H', 'e', 'l', 'l', 'o' };
        ReadOnlySpan<char> separator = stackalloc char[] { ' ' };
        ReadOnlySpan<char> part2 = stackalloc char[] { 'W', 'o', 'r', 'l', 'd' };
        Console.WriteLine($"\t{string.Concat(part1, separator, part2)}");

        ReadOnlySpan<string?> parts = [ "Hello", "World" ];
        Console.WriteLine($"\t{string.Join(", ", parts)}");

        // The MemoryExtensions class provides tons of useful extension methods
        // for spans and memory. Take the time to study them! Here is an example
        // of a brand new extension method for splitting a span.
        // https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.split?view=net-9.0#system-memoryextensions-split-1(system-readonlyspan((-0))-0)
        ReadOnlySpan<char> data = "25° C;36° F;16° C";
        var regex = new Regex(@"\d+°"); // Note: Use GeneratedRegex attribute in practice to speed up regex
                                        //       Here, we focus on spans, not regex -> we keep it simple
        foreach (var segmentRange in data.Split(';'))
        {
            var segment = data[segmentRange];
            int? degrees = null;
            foreach(var match in regex.EnumerateMatches(segment))
            {
                var value = segment[match.Index..(match.Index + match.Length)];
                degrees ??= int.Parse(value[..^1]);
            }

            Console.WriteLine($"\t{segmentRange.Start} - {segmentRange.End}: {segment} -> {degrees}");
        }

        // Here is another new extension method for span that checks if a span starts with a given value.
        ReadOnlySpan<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };
        Console.WriteLine($"\tDoes it start with 1? {numbers.StartsWith(1)}");
    }

    public static void BadIdeas()
    {
        Span<byte> ReturnSomeBytes()
        {
            // Note that we cannot return Span<T> if we use stackalloc
            Span<byte> bytes = /*stackalloc*/ new byte[9] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            return bytes;
        }

        PrintHeadline("Span returns:");
        foreach (var b in ReturnSomeBytes()) { Console.WriteLine("\t" + b); }
    }

    public static void UnmanagedBasics()
    {
        byte[] arr = [1, 2, 3, 5, 8, 13, 21, 34, 55];
        Span<byte> bytes = arr;

        // Allocate memory from unmanaged process heap
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        try
        {
            // Create Span<T> referencing unmanaged byte array
            Span<byte> gheapBytes;
            unsafe { gheapBytes = new Span<byte>((byte*)ptr, bytes.Length); }

            // Copy span into unmanaged memory
            bytes.CopyTo(gheapBytes);

            // Iterate over unmanaged heap that was filled from a Span<T>
            PrintHeadline("Iterate over unmanaged heap:");
            unsafe
            {
                var buffer = (byte*)ptr;
                for (var i = 0; i < bytes.Length; i++, buffer++)
                {
                    Console.WriteLine($"\t{*buffer}");
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public static void MemoryBasics()
    {
        static void DoSomethingWithSpan(Span<byte> bytes)
        {
            bytes[^1] = (byte)(bytes[^2] + bytes[^3]);
            foreach (var number in bytes) { Console.WriteLine(number); }
        }

        Memory<byte> bytes = new byte[] { 1, 2, 3, 0 };
        DoSomethingWithSpan(bytes.Span);
    }

    // Note that Color is a mutable(!) struct
    private struct Color
    {
        public byte R;
        public byte G;
#pragma warning disable CS0649 // Field is never assigned to
        public byte B;
#pragma warning restore CS0649 // Field is never assigned to
    }

    private struct RefColor(byte red)
    {
        public byte[] colors = [red, 0, 0];

        public ref byte R => ref colors[0]; // Do NOT make this readonly
    }

    public static void SpanVsList()
    {
        // Create an array of colors (structs)
        Color[] colorsArray = [new(), new() { G = 0xff }];

        // Create a list and a span from colors array
        List<Color> colorsList = [.. colorsArray];
        Span<Color> colorsSpan = colorsArray;

        // The following line will not work because left side is no variable to which you can assign a value
        //colorsList[0].R = 0xff;

        // Assignment works fine for Span<T>
        colorsSpan[0].R = 0xff;

        // How does Span do the trick? --> Ref returns
        RefColor[] refColorArray = [new RefColor(0xff), new RefColor(0)];
        var refColorsList = new List<RefColor>(refColorArray);
        refColorsList[0].R = 0xC0;
    }

    static void PrintHeadline(string text)
    {
        Console.WriteLine();
        Console.WriteLine(text);
        Console.WriteLine(new string('=', text.Length));
    }
}

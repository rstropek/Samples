using System.Runtime.InteropServices;

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
                        T temp = list[i + 1];
                        list[i + 1] = list[i];
                        list[i] = temp;
                        madeChanges = true;
                    }
                }
            } while (madeChanges);
        }

        void BubbleSortSpan<T>(Span<T> list) where T : IComparable
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

        // The problem:
        // We have a list of numbers and want to sort them
        byte[] numbers = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        BubbleSort(numbers);
        foreach (var n in numbers) { Console.WriteLine(n); }

        // Now we want to sort a part of the array -> we have to copy
        numbers = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        var someNumbers = new byte[numbers.Length - 2];
        for (var i = 1; i < numbers.Length - 1; i++) { someNumbers[i - 1] = numbers[i]; }
        BubbleSort(someNumbers);
        //foreach (var n in someNumbers) { Console.WriteLine(n); }

        // Span solves our problem
        Span<byte> numbersSpan = [55, 34, 21, 13, 8, 5, 3, 2, 1];
        BubbleSortSpan(numbersSpan[1..^1]);
        foreach (var n in numbersSpan) { Console.WriteLine(n); }
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
        ReadOnlySpan<char> answerSpan = answer.AsSpan();
        // Note that int.Parse accepts a ReadOnlySpan<char> as well
        Console.WriteLine("\t" + int.Parse(answerSpan[answerSpan.IndexOf('4')..]));
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
        public byte B;
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
        List<Color> colorsList = new(colorsArray);
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
        var oldBackground = Console.BackgroundColor;
        var oldForeground = Console.ForegroundColor;

        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);

        Console.BackgroundColor = oldBackground;
        Console.ForegroundColor = oldForeground;
    }
}

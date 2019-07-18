using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Span
{
    class Program
    {
        static void Main()
        {
            //TheProblem();

            //SpanBasics();
            //UnmanagedBasics();

            //BadIdeas();

            // Learning: With Span<T>, you can write methods supporting any kind of memory
            //           (including heap, stack, unmanaged memory).

            //SpanVsList();

            BenchmarkRunner.Run<StringsWithSpan>();

            //var stringsWithSpanTest = new StringsWithSpan { NumberOfLines = 100 };
            ////var result = stringsWithSpanTest.StringParsing(stringsWithSpanTest.GenerateHugeCsv(stringsWithSpanTest.NumberOfLines));
            //var result = stringsWithSpanTest.StringParsingWithSpan(stringsWithSpanTest.GenerateHugeCsvWithSpan(stringsWithSpanTest.NumberOfLines));
            //PrintHeadline("Age Statistic:");
            //for (var i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine($"{i * 10}..{(i + 1) * 10 - 1}: {result[i]}");
            //}
        }

        private static void TheProblem()
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
            var numbers = new byte[] { 55, 34, 21, 13, 8, 5, 3, 2, 1 };
            BubbleSort(numbers);
            foreach (var n in numbers) { Console.WriteLine(n); }

            // Now we want to sort a part of the array -> we have to copy
            numbers = new byte[] { 55, 34, 21, 13, 8, 5, 3, 2, 1 };
            var someNumbers = new byte[numbers.Length - 2];
            for (var i = 1; i < numbers.Length - 1; i++) { someNumbers[i - 1] = numbers[i]; }
            BubbleSort(someNumbers);
            //foreach (var n in someNumbers) { Console.WriteLine(n); }

            // Span solves our problem
            Span<byte> numbersSpan = new byte[] { 55, 34, 21, 13, 8, 5, 3, 2, 1 };
            BubbleSortSpan(numbersSpan[1..^1]);
            foreach (var n in numbersSpan) { Console.WriteLine(n); }
        }

        static void SpanBasics()
        {
            // Create a span based on an array (heap)
            var arr = new byte[] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            Span<byte> bytes = arr;

            // You can do the same on the stack
            // BTW: Note that stackalloc works with Span<T> even outside of unsafe context.
            //      Unfortunately, C# docs on stackalloc have not been updated at the time of writing.
            Span<byte> stackBytes = stackalloc byte[9] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };

            // Access element by index
            bytes[0] = 1;
            stackBytes[0] = 1;

            // Create a slice - which is a Span<t> itself - of the array
            // Note the use of a read-only span here.
            ReadOnlySpan<byte> slice = bytes.Slice(1, 2);

            // You can also use C# 8's new range feature
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
                // Note that this function uses the new "ref return" feature of C# 7.
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

            PrintHeadline("Spans and strings:");
            var answer = "The answer is 42";
            ReadOnlySpan<char> answerSpan = answer.AsSpan();
            Console.WriteLine("\t" + int.Parse(answerSpan.Slice(answerSpan.IndexOf('4'))));
        }

        static void BadIdeas()
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

        static void UnmanagedBasics()
        {
            var arr = new byte[] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };
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

        // Note that Color is a mutable(!) struct
        private struct Color
        {
            public byte R;
            public byte G;
            public byte B;
        }

        private struct RefColor
        {
            public byte[] colors;

            public RefColor(byte red)
            {
                colors = new[] { red, (byte)0, (byte)0 };
            }

            public ref byte R { get { return ref colors[0]; } }
        }

        static void SpanVsList()
        {
            // Create an array of colors (structs)
            var colorsArray = new[] { new Color(), new Color { G = 0xff } };

            // Create a list and a span from colors array
            var colorsList = new List<Color>(colorsArray);
            Span<Color> colorsSpan = colorsArray;

            // The following line will not work because left side is no variable to which you can assign a value
            //colorsList[0].R = 0xff;

            // Assignment works fine for Span<T>
            colorsSpan[0].R = 0xff;

            // How does Span do the trick? --> Ref returns
            var refColorArray = new[] { new RefColor(0xff), new RefColor(0) };
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
}

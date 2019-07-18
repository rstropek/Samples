using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

// Use dnSpy to check what index and range operations become

// Discuss the internal implementation of the Index struct.
// (https://github.com/dotnet/corefx/blob/release/3.0/src/Common/src/CoreLib/System/Index.cs)
// Especially note
// - Single member for from-beginning and from-end
// - Practical use of Span in Index.ToStringFromEnd

// Note that this feature will *NOT* work in .NET Framework 4.x.
// It needs .NET Standard 2.1 and .NET Framework 4.x. will not implement
// this new .NET Standard.

// Read the original proposal at https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/ranges.md

namespace RangesIndexes
{
    class Program
    {
        static void Main()
        {
            IndexingBasics();

            Ranges();
        }

        private static void IndexingBasics()
        {
            var numbers = new [] { 1, 2, 3, 4, 5 };

            // Get a single number using the new Index data type.
            // Note that from the left, it is zero-based.
            Index numberIndex = 1;
            var singleNumber = numbers[numberIndex];
            WriteLine(singleNumber);

            // Note that from the right, it is one-based.
            numberIndex = ^1;
            singleNumber = numbers[numberIndex];
            WriteLine(singleNumber);

            // Note that Index is a struct with two members:
            Index myIndex = new Index(1, true);
            if (myIndex.IsFromEnd)
            {
                WriteLine($"Index counts from the back {myIndex.Value} element(s)");
            }

            // Note that C# turns `numbers[numberIndex]` into something like that:
            singleNumber = numbers[myIndex.IsFromEnd ? (numbers.Length - myIndex.Value) : myIndex.Value];

            // You can turn integers into Index...
            var myIntIndex = 1;
            myIndex = myIntIndex;
            // ...but not the other way round. Use myIndex.Value to get int index
            // myIntIndex = myIndex; --> Error
        }

        private static void Ranges()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };

            // Use an index operator to get a slice of the array
            var numbersSplice = numbers[1..4];
            WriteLine(numbersSplice.Aggregate(string.Empty, (acc, n) => acc += $" {n}"));

            numbersSplice = numbers[1..^1];
            WriteLine(numbersSplice.Aggregate(string.Empty, (acc, n) => acc += $" {n}"));

            // Ranges can be open-ended
            numbersSplice = numbers[..^2]; // Skip last two elements
            numbersSplice = numbers[1..]; // Skip first element
            numbersSplice = numbers[..]; // Get all elements

            // Note that the range operator applied on an array uses
            // Array.Copy in the background.

            // Works with Index type, too
            Index numberIndex = ^1;
            numbersSplice = numbers[1..numberIndex];
            WriteLine(numbersSplice.Aggregate(string.Empty, (acc, n) => acc += $" {n}"));

            // Range is a struct with two Indexes
            Range myRange = new Range(1, ^1);
            Range myRange2 = 1..^1;
            WriteLine($"This range is from {myRange.Start} to {myRange.End}");

            // Currently, ranges do not work with Lists
            var numberList = new List<int> { 1, 2, 3, 4, 5 };
            // var sublist = numberList[1..^2]; --> Error

            // You can apply ranges to strings. In that case, it becomes a call to Substring
            var letters = "abcdefg";
            WriteLine(letters[1..^2]);

            // Note that spans implement support for indexes and ranges, too
            var numbersSpan = numbers.AsSpan()[1..^1];
            foreach(var n in numbersSpan)
            {
                Write(" " + n);
            }
            WriteLine();
        }
    }
}

using System;
using System.Collections.Generic;

// Recap: Value types vs. reference types

// Recap: What is "int?" -> Nullable<int> (look it up in source.dot.net)
// Note: Nullable reference types are NOT Nullable<T>! They are just "compiler magic".
//       a string? is still a string.

// Note that the nullable annotation context is enabled in .csproj for this project.
// (see also https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references#nullable-contexts)

namespace NullableReferenceTypes
{
    class Program
    {

        static void Main()
        {
            // Note that myString must not be null
            string myString = DateTime.Today.Day % 2 == 0 ? null : "Yes";
            PrintLength(myString); // Try myString! instead
            SafePrintLength(myString);

            // Note nullable reference type
            string? myNullableString = DateTime.Today.Day > 31 ? null : "Yes";

            // In this case we are smarter than the compiler. Although the string
            // can be null, we know that logically it will never be null. Therefore,
            // we can use the new !. dereferencing operator.
            Console.WriteLine(myNullableString!.Length);

#nullable disable
            myString = DateTime.Today.Day % 2 == 0 ? null : "Yes";
            PrintLength(myString);
            SafePrintLength(myString);
#nullable restore

            var customerDictionary = new Dictionary<int, string>
            {
                { 1, "Foo" },
                { 2, "Bar" }
            };
            customerDictionary.TryGetValue(1, out var c);

            // Try this with "if" and find out how the compiler understands
            // nullability by going to definition of "TryGetValue".
            // See also https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis?view=netcore-3.1
            // if (customerDictionary.TryGetValue(1, out var c))
            {
                Console.WriteLine(c.Length);
            }

            // Discuss pre/postconditions from
            // https://devblogs.microsoft.com/dotnet/try-out-nullable-reference-types/?WT.mc_id=ondotnet-c9-cxa

            Console.WriteLine(SumLengthOfToString(new[] { 1, 2, 3 }));
            Console.WriteLine(SumLengthOfToString(new[] { "1", "2", "3" /*, null */ }));
        }

        static void RiskyPrintLength(string? s)
        {
            Console.WriteLine(s!.Length);
        }

        static void PrintLength(string s)
        {
            Console.WriteLine(s.Length);
        }

        static void SafePrintLength(string? s)
        {
            if (s != null)
            {
                Console.WriteLine(s.Length);
            }
        }

        // Note: New "notnull" generic constraint
        static  int SumLengthOfToString<T>(IEnumerable<T> items) // where T: notnull
        {
            var sum = 0;
            foreach (var item in items)
            {
                sum += item.ToString().Length;
                // sum += item.ToString()?.Length ?? 0;
            }

            return sum;
        }
    }
}

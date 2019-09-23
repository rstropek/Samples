using System;

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
    }
}

using System;

namespace OutVars
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Please enter a number: ");
            var numberAsString = Console.ReadLine();

            Classic(numberAsString);
            CSharpSeven(numberAsString);
        }

        static void Classic(string numberAsString)
        {
            int number;
            if (int.TryParse(numberAsString, out number))
            {
                Console.WriteLine($"Ok, I got the number {number}");
            }
            else
            {
                Console.WriteLine("Sorry, this is not a number");
            }
        }

        static void CSharpSeven(string numberAsString)
        {
            // Note the use of `out var` here. In IL, this code is identical to the
            // classic version shown above. More infos about `out var` at
            // https://docs.microsoft.com/en-us/dotnet/articles/csharp/whats-new/csharp-7#out-variables
            if (int.TryParse(numberAsString, out var number))
            {
                Console.WriteLine($"Ok, I got the number {number}");
            }
            else
            {
                Console.WriteLine("Sorry, this is not a number");
            }
        }

        static void ReturnSomeNumbers(out int val1, out int val2, out int val3) => val1 = val2 = val3 = 42;

        static void ConsumeSomeNumbers()
        {
            // Note that use of `_` here to indicate that you are not interested in
            // some of the out-variables.
            ReturnSomeNumbers(out var val, out _, out _);
            Console.WriteLine(val);
        }

        static void ConsumeSomeNumbers2()
        {
            ReturnSomeNumbers(out var val, out var dummy1, out var dummy2);
            Console.WriteLine(val);
        }
    }
}

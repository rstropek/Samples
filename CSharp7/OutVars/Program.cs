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
    }
}

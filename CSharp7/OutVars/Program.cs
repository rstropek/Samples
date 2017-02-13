using System;

namespace OutVars
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Please enter a number: ");
            var numberAsString = Console.ReadLine();
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

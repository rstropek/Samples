using System;
using System.Diagnostics;
using System.Threading.Tasks;

// Read more about local functions at
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/local-functions

// Note that C# 8 will add static local functions
// (https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#static-local-functions).
// They cannot capture (reference) any variables from the enclosing scope.

namespace LocalFunction
{
    delegate int MathOp(int a, int b);

    class Program
    {
        static void Main()
        {
            BasicLocalFunction();
            SumFromZeroTo100();
            AsyncLocalFunction();
            Closure();
            Console.WriteLine($"The result is {ReturnClosure()(1, 2)}\n");
            new Program().ManyCalculations();
        }

        /// <summary>
        /// Very simple local function
        /// </summary>
        static void BasicLocalFunction()
        {
            int Add(int x, int y) => x + y;
            Console.WriteLine($"The result is {Add(1, 2)}\n");
        }

        /// <summary>
        /// Local function defined after being called
        /// </summary>
        static void SumFromZeroTo100()
        {
            Console.WriteLine($"The sum is {LocalSum(100)}\n");

            // Note that function is defined after being called
            int LocalSum(int value) => value == 1 ? 1 : value + LocalSum(value - 1);
        }

        /// <summary>
        /// Simple asynchronous function
        /// </summary>
        static void AsyncLocalFunction()
        {
            async Task<int> AddAsync(int x, int y)
            {
                await Task.Delay(1);
                return x + y;
            };
            Console.WriteLine($"The result is {AddAsync(1, 2).Result}\n");
        }

        /// <summary>
        /// Simple local function with closure
        /// </summary>
        static void Closure()
        {
            var rand = new Random();
            var randomValue = rand.Next(0, 10);

            // Note how add is using the local variable "randomValue"
            int Add(int x, int y) => x + y + randomValue;
            Console.WriteLine($"The result is {Add(1, 2)}");

            randomValue = rand.Next(0, 10);
            Console.WriteLine($"The second result is {Add(1, 2)}\n");
        }

        /// <summary>
        /// Return local function with closure
        /// </summary>
        static MathOp ReturnClosure()
        {
            var rand = new Random();
            var randomValue = rand.Next(0, 10);

            // Note how the following function is using the local variable "randomValue"
            int Add(int x, int y) => x + y + randomValue;
            return Add;
        }

        private readonly int factor = 42;

        void ManyCalculations()
        {
            const int seconds = 10;
            var rand = new Random();
            var watch = new Stopwatch();
            var counter = 0L;

            Console.WriteLine($"Calculating for {seconds} seconds...");
            watch.Start();
            while (watch.Elapsed < TimeSpan.FromSeconds(seconds))
            {
                // Note that Add uses "factor"
                int Add(int x, int y) => x + y + factor;

                // Note discard in the next statement. Read more at
                // https://docs.microsoft.com/en-us/dotnet/csharp/discards
                _ = Add(rand.Next(0, 100), rand.Next(0, 100));
                counter++;
            }

            Console.WriteLine($"Done {counter} calculations");
        }
    }
}

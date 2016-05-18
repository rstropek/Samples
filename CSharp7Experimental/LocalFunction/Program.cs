using System;
using System.Diagnostics;
using System.Threading.Tasks;

/*
 * Note that this sample demonstrates C# features that are currently under
 * development. Read more about Local Function feature status on GitHub
 * at https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md.
 */

namespace LocalFunctionLambda
{
    delegate int MathOp(int a, int b);

    class Program
    {
        static void Main(string[] args)
        {
            BasicLocalFunction();
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

        private int factor = 42;

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
                var result = Add(rand.Next(0, 100), rand.Next(0, 100));
                counter++;
            }

            Console.WriteLine($"Done {counter} calculations");
        }
    }
}

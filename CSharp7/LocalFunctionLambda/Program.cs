using System;
using System.Diagnostics;
using System.Threading.Tasks;

#pragma warning disable IDE0039 // Use local function

// This sample demonstrates local functions implemented with lambdas. This is
// not new in VS 2017, I just use it as an introduction to local functions
// (see project in ../LocalFunction).

namespace LocalFunctionLambda
{
    delegate int MathOp(int a, int b);

    class Program
    {
        static void Main()
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
            // Note that VS suggests to use a local function instead of 
            // the lambda function.
            MathOp add = (x, y) => x + y;
            Console.WriteLine($"The result is {add(1, 2)}\n");
        }

        /// <summary>
        /// Simple asynchronous function
        /// </summary>
        static void AsyncLocalFunction()
        {
            Func<int, int, Task<int>> addAsync = async (x, y) =>
            {
                await Task.Delay(1);
                return x + y;
            };
            Console.WriteLine($"The result is {addAsync(1, 2).Result}\n");
        }

        /// <summary>
        /// Simple local function with closure
        /// </summary>
        static void Closure()
        {
            var rand = new Random();
            var randomValue = rand.Next(0, 10);

            // Note how add is using the local variable "randomValue"
            MathOp add = (x, y) => x + y + randomValue;
            Console.WriteLine($"The result is {add(1, 2)}");

            randomValue = rand.Next(0, 10);
            Console.WriteLine($"The second result is {add(1, 2)}\n");
        }

        /// <summary>
        /// Return local function with closure
        /// </summary>
        static MathOp ReturnClosure()
        {
            var rand = new Random();
            var randomValue = rand.Next(0, 10);

            // Note how the following lambda is using the local variable "randomValue"
            return (x, y) => x + y + randomValue;
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
                // Note that add uses "factor"
                MathOp add = (x, y) => x + y + factor;
                _ = add(rand.Next(0, 100), rand.Next(0, 100));
                counter++;
            }

            Console.WriteLine($"Done {counter} calculations");
        }
    }
}

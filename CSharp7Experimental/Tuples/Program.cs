using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        static readonly (int sum, int count) staticLiteral = (0, 1);

        public static void Main(string[] args)
        {
            var numbers = new [] { 1, 2, 3, 4, 5 };
            
            // Old syntax
            int localSum, localCount;
            Analyze(numbers, out localSum, out localCount);
            Console.WriteLine($"Sum: {localSum}, Count: {localCount}");
            
            // New syntax
            var result = Analyze(numbers);
            Console.WriteLine($"Sum: {result.sum}, Count: {result.count}");

            // Tuple literals
            var literal = new (int sum, int count) { sum = 0, count = 1 };
            literal = (1, 2);
            Console.WriteLine($"Sum: {literal.sum}, Count: {literal.count}");
            result = literal; // Note that you can assign tuples
            Console.WriteLine($"Sum: {result.sum}, Count: {result.count}");
            var literal2 = (3, 4);
            Console.WriteLine($"Sum: {literal2.Item1}, Count: {literal2.Item2}");

            // Note that tuples are mutable, you can make them readonly
            literal.sum = 42;
            Console.WriteLine($"Sum: {literal.sum}, Count: {literal.count}");
            // staticLiteral.sum = 42; -> would result in an error

            // Tuples in Lambdas
            Func<(int sum, int count)> f = () => (1, 2);
            Console.WriteLine(f().sum);

            // Tuples with Tasks
            Task<(int sum, int count)> t = Task.FromResult(f());
            Console.WriteLine(t.Result);

            // Tuples and generic collections
            IEnumerable<(int sum, int count)> collection = new [] { (0, 0), (1, 1) };
            foreach(var item in collection)
            {
                Console.Write($"{item}\t");
            }
        }

        public static void Analyze(IEnumerable<int> numbers, out int sum, out int count)
        {
            sum = numbers.Sum();
            count = numbers.Count();
        }

        public static (int sum, int count) Analyze(IEnumerable<int> numbers) =>
            (numbers.Sum(), numbers.Count());
    }
}

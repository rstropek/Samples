using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

// Note that you have to install System.ValueTuple for this sample (Preview 4)

namespace ConsoleApplication
{
    public class Program
    {
        static readonly (int sum, int count) staticLiteral = (0, 1);

        public static void Main(string[] args)
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };

            #region Old syntax
            void AnalyzeWithOutParams(out int sum, out int count)
            {
                sum = numbers.Sum();
                count = numbers.Count();
            }

            int localSum, localCount;
            AnalyzeWithOutParams(out localSum, out localCount);
            Console.WriteLine($"Sum: {localSum}, Count: {localCount}");

            Tuple<int, int> AnalyzeWithTuple()
            {
                return new Tuple<int, int>(numbers.Sum(), numbers.Count());
            }

            var oldTupleResult = AnalyzeWithTuple();
            Console.WriteLine($"Sum: {oldTupleResult.Item1}, Count: {oldTupleResult.Item2}");
            #endregion

            #region New syntax
            (int sum, int count) Analyze() => (numbers.Sum(), numbers.Count());

            (int, int) Analyze2() => (numbers.Sum(), numbers.Count());

            var result = Analyze();
            Console.WriteLine($"Sum: {result.sum}, Count: {result.count}");
            var result2 = Analyze2();
            Console.WriteLine($"Sum: {result.Item1}, Count: {result.Item2}");
            ValueTuple<int, int> result3 = Analyze();
            Console.WriteLine($"Sum: {result3.Item1}, Count: {result3.Item2}");

            // Deconstruction
            (var mySum, var myCount) = Analyze();
            // Alternate syntaxes:
            // (int mySum, int myCount) = Analyze(numbers);
            // var (mySum, myCount) = Analyze(numbers);
            // int mySum, myCount; (mySum, myCount) = Analyze(numbers);
            Console.WriteLine($"Sum: {mySum}, Count: {myCount}");

            // Tuple literals
            (int sum, int count) literalExplicitType = (sum: 0, count: 1);
            var literal = (sum: 0, count: 1);
            literal = (1, 2);
            (int, int) literalWithoutElementNames = (2, 3);
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
            IEnumerable<(int sum, int count)> collection = new[] { (0, 0), (1, 1) };
            foreach (var item in collection)
            {
                Console.Write($"{item}\t");
            }

            Console.WriteLine();
            #endregion

            // Deconstructors
            var p = new Point(1, 2);
            var (x, y) = p;
            Console.WriteLine($"p.x={x}, p.y={y}");
        }
    }

    class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y) { X = x; Y = y; }

        // Note deconstructor function here
        public void Deconstruct(out int x, out int y) { x = X; y = Y; }
    }
}

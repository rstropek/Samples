using System;
using System.Collections.Generic;

/*
 * Note that this sample demonstrates C# features that are currently under
 * development. Read more about Pattern Matching feature status on GitHub
 * at https://github.com/dotnet/roslyn/issues/10866.
 */

namespace PatternMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Type Pattern
            object o = "foo";
            if (o is string name)
            {
                Console.WriteLine($"o is a string with value {name}");
            }

            // Note the use of separators in the following collection initializer
            IEnumerable<int> numbers = new List<int> { 1_024, 0b1000_0000_0000, 0x10_00 };
            if (numbers is IList<int> numberList)
            {
                Console.WriteLine("We have a list of ints");
                numberList.Add(4);
            }

            switch (o)
            {
                case string name:
                    Console.WriteLine($"o is a string with value {name}");
                    break;
                case double number when number > 100.0:
                    Console.WriteLine("Number is a high double");
                    break;
                case var obj:
                    Console.WriteLine($"Got {obj.ToString()}");
                    break;
            }
            #endregion

            #region Type Pattern Null Check
            var customers = new[]
            {
                new { Name = "Foo", Orders = new [] { new { ID = 1, Revenue = 42.0 }} },
                new { Name = "Bar", Orders = new [] { new { ID = 1, Revenue = 41.0 }} }
            };

            double? someNullableResult = customers?[0]?.Orders?[0]?.Revenue;
            if (someNullableResult is double result)
            {
                Console.WriteLine($"The result is {result}");
            }

            if (someNullableResult is double result && result < 100.0)
            {
                Console.WriteLine($"The result is a low result");
            }

            #endregion

            #region Constant Pattern
            object something = 42;
            if (something is 42)
            {
                Console.WriteLine("Something is 42");
            }
            #endregion

            #region Var Pattern
            something = "foobar";
            if (something is var x)
            {
                Console.WriteLine(x); // prints "foobar"
            }
            #endregion
        }
    }
}

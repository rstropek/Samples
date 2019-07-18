using System;
using System.Collections.Generic;
using System.Threading;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0019 // Use pattern matching

// Learn more about C# pattern matching at https://docs.microsoft.com/en-us/dotnet/articles/csharp/pattern-matching.

namespace PatternMatching
{
    abstract class Animal { }

    class Dog : Animal { }
    class Cat : Animal { }

    class Program
    {
        static void Main()
        {
            PerfTestIsPattern();

            #region Type Pattern
            object o = "foo";

            void CheckClassic(object obj)
            {
                // In the past we would have written...
                var name = obj as string;
                if (name != null)
                {
                    Console.WriteLine($"o is a string with value {name}");
                }
            }

            void CheckCSharpSeven(object obj)
            {
                // Now we can be much more concise using a "Type Pattern":
                if (obj is string name)
                {
                    // Now we have a local variable `name` of type `string`
                    Console.WriteLine($"o is a string with value {name}");
                }
            }

            CheckClassic(o);
            CheckCSharpSeven(o);

            // Note that the local variable `name` exists even here outside of 
            // the `if` block. So you can no longer use the variable name `name`.

            // Note the use of separators in the following collection initializer
            IEnumerable<int> numbers = new List<int> { 1_024, 0b1000_0000_0000, 0x10_00 };
            if (numbers is IList<int> numberList)
            {
                Console.WriteLine("We have a list of ints");
                numberList.Add(4);
            }

            Animal animal = new Dog();
            if (animal is Dog dog)
            {
                Console.WriteLine("We have a dog...");
            }

            // Note that we can use the Type Pattern in `switch`, too.
            switch (o)
            {
                // Note that ordering matters. `foo` has to be checked before the
                // following case statement (`string objectName´)
                case "foo":
                    Console.WriteLine("This is foo.");
                    break;

                case string objectName:
                    Console.WriteLine($"o is a string with value {objectName}");
                    break;

                // Note the case guard here.
                case double number when number > 100.0:
                    Console.WriteLine("Number is a very high double");
                    break; // Note that it is not possible to omit the `break` statement

                // Note that ordering matters again
                case double number when number > 50.0:
                    Console.WriteLine("Number is a high double");
                    break;

                case null:
                    // Add a null case to ensure the argument is not null
                    throw new ArgumentNullException("error...");

                // Note that "Var Pattern" here. It is always true.
                // You could add a `when` clause to do something useful.
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

            // Recap: Can you remember null conditional operator in C#?
            // Read more at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-conditional-operators
            double? someNullableResult = customers?[0]?.Orders?[0]?.Revenue;
            if (someNullableResult is double result)
            {
                Console.WriteLine($"The result is {result}");
            }

            if (someNullableResult is double numResult && numResult < 100.0)
            {
                Console.WriteLine($"The result is a low result");
            }

            #endregion

            #region Constant Pattern
            object something = 42;

            // The following statement would not work (you cannot use == with
            // object and int). You would have to use `Equals` instead.
            // if (something == 42) ...

            // Now we can write this much nicer:
            if (something is 42)
            {
                Console.WriteLine("Something is 42");
            }

            // Pattern with out variable
            if (something is int someNumber || int.TryParse(something.ToString(), out someNumber))
            {
                Console.WriteLine($"I got the number {someNumber}");
            }
            #endregion

            #region Var Pattern
            something = "foobar";
            if (something is var x)
            {
                Console.WriteLine(x); // prints "foobar"
            }
            #endregion

            SwitchWithPatternDemo();
        }

        public static void SwitchWithPatternDemo()
        {
            object o = 42d;

            switch (o)
            {
                case double d when d > 10:
                    Console.WriteLine("It's greater 10");
                    break;

                case double d when d > 20:
                    Console.WriteLine("It's greater 20");
                    break;
            }
        }

        public static void PerfTestIsPattern()
        {
            string CheckClassic(object obj)
            {
                var name = obj as string;
                if (name != null)
                {
                    return name;
                }

                return null;
            }

            string CheckCSharpSeven(object obj) => (obj is string name) ? name : null;

            long RunAndCount(Action body)
            {
                var counter = 0L;
                var stopRequested = false;
                var timer = new Timer(_ => stopRequested = true,
                                      null,
                                      TimeSpan.FromSeconds(10),
                                      new TimeSpan(-1));
                while (!stopRequested)
                {
                    body();
                    counter++;
                }

                Console.WriteLine($"Executed {counter} times.");
                return counter;
            }

            var rand = new Random();
            var classicCounter = (double)RunAndCount(() => CheckClassic(rand.Next(2) == 0 ? "foo" : (object)rand));
            var csharp7Counter = (double)RunAndCount(() => CheckCSharpSeven(rand.Next(2) == 0 ? "foo" : (object)rand));
            Console.WriteLine($"Classic: {classicCounter}, C# 7: {csharp7Counter}, Advantage: {(csharp7Counter / classicCounter - 1) * 100}%");
        }
    }
}

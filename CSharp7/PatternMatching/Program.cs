using System;
using System.Collections.Generic;

namespace PatternMatching
{
    abstract class Animal { }

    class Dog : Animal {  }
    class Cat : Animal { }

    class Program
    {
        static void Main(string[] args)
        {
            #region Type Pattern
            object o = "foo";

            // In the past we would have written...
            var objName = o as string;
            if (objName != null)
            {
                Console.WriteLine($"o is a string with value {objName}");
            }

            // Now we can be much more concise using a "Type Pattern":
            if (o is string name)
            {
                // Now we have a local variable `name` of type `string`
                Console.WriteLine($"o is a string with value {name}");
            }

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
                case string objectName:
                    Console.WriteLine($"o is a string with value {objectName}");
                    break;
                
                // Note the case guard here.
                case double number when number > 100.0:
                    Console.WriteLine("Number is a high double");
                    break;

                // Note that "Var Pattern" here. It is always true.
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
        }
    }
}

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ref
{
    struct TwoDoubles
    {
        public double A;

        public double B;

        public override string ToString()
        {
            return $"{{ \"A\": \"{A}\", \"B\": \"{B}\" }}";
        }

        // Create an immutable instance of TwoDoubles by returning
        // it using ref readonly. This is a new feature of C# 7.
        // Read more at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref#ref-readonly-locals
        private static TwoDoubles empty = new TwoDoubles { A = 0, B = 0 };
        public static ref readonly TwoDoubles Empty => ref empty;
        public void SetToDefault()
        {
            A = 1;
            B = A * (-1);
        }
    }

    // The following struct must be immutable because it is readonly.
    // Read more at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/readonly#readonly-struct-example
    readonly struct ReadOnlyTwoDoubles
    {
        public double A { get; }

        public double B { get; }

        public ReadOnlyTwoDoubles(double a, double b)
        {
            A = a;
            B = b;
        }

        public override string ToString()
        {
            return $"{{ \"A\": \"{A}\", \"B\": \"{B}\" }}";
        }
    }

    struct ArrayOfDoubles
    {
        public double[] Values;

        // Note ref return. This is a new feature in C# 7.
        // Read more at https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns
        public ref double this[int index]
        {
            // Note the throw expression here. Read more at
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression
            get { return ref (Values ?? throw new InvalidOperationException("Values is null"))[index]; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder("[ ");

            // Note the throw expression here. This is a new feature in C# 7.
            // Read more at
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression
            foreach (var value in Values ?? throw new InvalidOperationException("Values is null"))
            {
                if (builder.Length > 2)
                {
                    builder.Append(", ");
                }

                builder.Append(value);
            }

            builder.Append(" ]");

            return builder.ToString();
        }
    }

    class Program
    {
        static void Main()
        {
            // val will be copied
            var val = new TwoDoubles { A = 1d, B = -1d };
            var swappedVal = Swap(val);
            Console.WriteLine($"{val} swapped is {swappedVal}");

            // Address of val will be passed
            val = new TwoDoubles { A = 1d, B = -1d };
            swappedVal = SwapWithRef(ref val);
            Console.WriteLine($"{val} swapped is {swappedVal}");

            // Again, address of val will be passed but method cannot change val.
            // "in" is a new feature of C# 7.2. Read more at
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-parameter-modifier
            val = new TwoDoubles { A = 1d, B = -1d };
            swappedVal = SwapWithIn(in val); // Note that you can skip "in" here
            Console.WriteLine($"{val} swapped is {swappedVal}");

            // Use a ref local variable to swap values in-place. This is
            // a new feature in C# 7.
            val = new TwoDoubles { A = 1d, B = -1d };
            ref TwoDoubles inPlaceSwapped = ref SwapInPlace(ref val);
            Console.WriteLine($"{val} swapped is {inPlaceSwapped} - this is WRONG!");

            // Demonstrate throw expression
            try { Console.WriteLine(new ArrayOfDoubles()); }
            catch (InvalidOperationException) { Console.Error.WriteLine("Exception!"); }

            // Demonstrate ref return with indexer
            var vals = new ArrayOfDoubles { Values = new[] { 1d, -1d } };
            Console.Write($"{vals} swapped is ");
            (vals[0], vals[1]) = (vals[1], vals[0]);
            Console.WriteLine(vals);

            // The following line creates a copy of TwoDouble.Empty because left side
            // is not a ref readonly. As a result, we can change val in any way we want.
            val = TwoDoubles.Empty;
            val.A = 42d;
            val.SetToDefault();
            Console.WriteLine($"This is a default instance: {val}");

            // By declaring the left side ref readonly, we get an immutable instance.
            // It cannot be changed (e.g. "valRef.A = 1" does not work). If we call a
            // method on valRef, C# will create a copy to make sure that valRef is not changed.
            ref readonly TwoDoubles valRef = ref TwoDoubles.Empty;
            Console.WriteLine($"This is an empty instance: {valRef}");
            valRef.SetToDefault();
            Console.WriteLine($"Calling a manipulating method does not change it: {valRef}");

            // Demonstrate ref structs
            WrongParallelFixAsync().Wait();
        }

        static TwoDoubles Swap(TwoDoubles source)
        {
            //// For demo purposes, lets change source
            //source.A += 1;
            //source.B -= 1;

            return new TwoDoubles { A = source.B, B = source.A };
        }

        static TwoDoubles SwapWithRef(ref TwoDoubles source)
        {
            //// For demo purposes, lets change source
            //source.A += 1;
            //source.B -= 1;

            return new TwoDoubles { A = source.B, B = source.A };
        }

        static TwoDoubles SwapWithIn(in TwoDoubles source)
        {
            // Cannot change source because it is "in" keyword

            return new TwoDoubles { A = source.B, B = source.A };
        }

        static ref TwoDoubles SwapInPlace(ref TwoDoubles source)
        {
            // Note swapping variables using C# 7's tuple syntax
            (source.A, source.B) = (source.B, source.A);

            // Note C# 7's new ref return statement. This will return a reference to
            // the original variable "source".
            return ref source;
        }

        static async Task WrongParallelFixAsync()
        {
            TwoDoubles val;
            async Task Fix()
            {
                // By accessing val in this method, it is captured in a "Display Class"
                // (see dnSpy). Therefore, it is promoted to the managed heap. As a result,
                // access to val is not threadsafe although it is a struct.

                // Make sure that A + B == 0
                if (val.A + val.B != 0)
                {
                    var helper = val.A;
                    await Task.Delay(1); // Simulate some async activity
                    val.B = helper * (-1);
                }
            }

            async Task Fix2()
            {
                // Make sure that A + B == 0
                if (val.A + val.B != 0)
                {
                    var helper = val.B;
                    await Task.Delay(1); // Simulate some async activity
                    val.A = helper * (-1);
                }
            }

            val = new TwoDoubles { A = 1, B = 1 };
            await Task.WhenAll(Fix(), Fix2());
            Console.WriteLine($"The result of parallel manipulation is WRONG: {val}");
        }

        // Note that this struct must be stack allocated. It cannot be promoted to the
        // managed heap. To ensure that, a lot of rules apply (for details see
        // https://docs.microsoft.com/en-us/dotnet/csharp/reference-semantics-with-value-types#ref-struct-type).
        // Try replacing "TwoDoubles" in the above example with "TwoDoublesOnStack".
        // It will - by design - not work.
        ref struct TwoDoublesOnStack
        {
            public double A;

            public double B;
        }

        // Note that C# 8 will add disposable ref structs
        // (https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#disposable-ref-structs).
    }
}

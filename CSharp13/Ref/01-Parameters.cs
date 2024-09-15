namespace Ref;

public static class Parameters
{
    record struct LargeStruct(int X, int Y, int Z);

    static void DoSomething(ref int x, ref LargeStruct largeStruct)
    {
        // Note that we are modifying the value of x. This is possible
        // because we are using the ref keyword as a parameter modifier.
        // This leads to passing the argument by reference.
        x += 5;

        // We can also modify the value of the struct because we are passing
        // it by reference.
        largeStruct.X += 5;
    }


    static void ReadRef(ref readonly int x, ref readonly LargeStruct largeStruct)
    {
        // Note: ref readonly forces the compiler to pass the argument
        // by reference. This is useful when you want to pass large
        // structs by reference without the risk of modifying them.

        // x += 5; // This is not allowed because x is readonly
        Console.WriteLine(x);

        // largeStruct.X += 5; // This is not allowed because largeStruct is readonly
        Console.WriteLine(largeStruct.X);
    }

    static readonly int y = 10;

    public static void RefParameters()
    {
        int x = 10;
        LargeStruct largeStruct = new(1, 2, 3);

         // That that the caller must "agree" to use the ref keyword
        DoSomething(ref x, ref largeStruct);
        Console.WriteLine(x);

        // Works like before; no "readonly" keyword is needed on the caller side
        ReadRef(ref x, ref largeStruct);

#pragma warning disable CS0219 // Variable is assigned but its value is never used
        const int z = 10;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

        // ReadRef(ref y); // Does not compile because y is readonly
        ReadRef(in y, in largeStruct); // This is allowed for ref readonly parameters
        // ReadRef(in z); // Does not compile because z is const
    }

    // What other parameter modifiers do you know?
    #region Parameter modifiers
    // out - for multiple return values
    // in - allows (not forces) the compiler to pass a readonly reference to a variable.
    //      Particularly useful when passing large structs to methods.
    // params - allows a method to accept a variable number of arguments
    #endregion
}
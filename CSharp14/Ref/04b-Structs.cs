namespace Ref;

// Note: ref structs MUST LIVE ON THE STACK
ref struct Vector3
{
    public float X;
    public float Y;
    public float Z;
}

// struct Line
// {
//     public Vector3 Start; // Not ok because Line could be allocated on the heap
//     public Vector3 End;
// }

ref struct Line
{
#pragma warning disable CS0649 // Field is never assigned to
    public Vector3 Start; // This is ok because Line is a ref struct itself
    public Vector3 End;
#pragma warning restore CS0649 // Field is never assigned to
}

public static class RefStructs
{
    public static void RefStruct()
    {
        var v1 = new Vector3 { X = 1, Y = 2, Z = 3 }; // Ok because memory is allocated on the stack

        //object o1 = (object)v1; // Not ok because of boxing -> memory would be allocated on the heap
        //Vector3[] a1 = [ v1 ]; // Not ok because arrays are reference types -> memory would be allocated on the heap
        // Print(() => Console.WriteLine(v1.ToString())); // Not ok because ref structs cannot be captured in a closure
    }

    static void Print(Action print)
    {
        print();
    }
}

// Question: Why is it important that Span is a ref struct?
#region Answer
// A ref struct can only be allocated on the stack, not on the heap. This is crucial for 
// Span<T> because it often points to stack-allocated memory, such as local variables or 
// spans of stack-allocated arrays. If Span<T> could be allocated on the heap, it could 
// outlive the stack memory it's pointing to, leading to undefined behavior, memory 
// corruption, or access violations.
#endregion

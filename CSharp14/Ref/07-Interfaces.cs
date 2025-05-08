using System.Numerics;
using System.Runtime.InteropServices;

namespace Ref;

// In C# 13, generic types can explicitly allow ref structs.
// The compiler enforces ref safety rules.
interface IAddable<T> where T : allows ref struct
{
    static abstract T Add(T left, T right);
}

// In C# 13, ref structs can implement interfaces.
ref struct Vector2(float x, float y) : IAddable<Vector2>
{
    public float X = x;
    public float Y = y;

    public static Vector2 Add(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);
}

// Quiz: Can C#'s `using` only be used with types that implement IDisposable directly or indirectly?
#region Answer
// No, as long as the type has the necessary members, it can be used with the `using` statement.
// This is called duck-typing or structural typing.
#endregion

ref struct UnmanagedData : IDisposable
{
    private unsafe void* _unmanagedMemory;
    private Span<byte> _unmanagedSpan;
    private const int MemorySize = 1024; // Allocate 1KB of unmanaged memory

    public unsafe UnmanagedData()
    {
        _unmanagedMemory = NativeMemory.Alloc(MemorySize);
        _unmanagedSpan = new Span<byte>(_unmanagedMemory, MemorySize);
    }

    public ref byte this[int index] => ref _unmanagedSpan[index];

    // struct has a Dispose method, so it can be used with the `using` statement.
    // However, starting with C# 13, it can also implement IDisposable.
    public unsafe void Dispose()
    {
        if (_unmanagedMemory != null)
        {
            Console.WriteLine("Disposing of unmanaged data");
            NativeMemory.Free(_unmanagedMemory);
            _unmanagedMemory = null;
        }
    }
}

public static class RefInterfaces
{
    public static void Interfaces()
    {
        Vector2 v1 = new(1, 2);
        // var addable = v1 as IAddable<Vector2>; // This is not allowed as it would lead to boxing.
        Vector2 v2 = new(3, 4);
        Vector2 v3 = Vector2.Add(v1, v2);

        Console.WriteLine($"{v3.X}, {v3.Y}");

        using (UnmanagedData data = new())
        {
            Console.WriteLine("Unmanaged data created");

            data[0] = 1;
            data[0]++;
            Console.WriteLine(data[0]);
        }

        var agg = new Aggregator<AnotherVector2>();
        agg.Add(new AnotherVector2(1, 2));
        agg.Add(new AnotherVector2(3, 4));
        
        Console.WriteLine(agg.TotalLength);
    }
}

interface IHaveLength
{
    float GetLength();
}

// ref struct implementing interface is new in C# 13.
ref struct AnotherVector2(float x, float y) : IHaveLength
{
    public float X = x;
    public float Y = y;

    public float GetLength() => MathF.Sqrt(X * X + Y * Y);
}

class Aggregator<T> where T: IHaveLength, allows ref struct {
    public float TotalLength { get; private set; }

    public void Add(T item) => TotalLength += item.GetLength();
}
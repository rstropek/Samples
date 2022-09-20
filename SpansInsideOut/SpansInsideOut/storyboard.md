# Spans Inside Out

## Starter Code

```csharp
#region Sort algorithms
static void BubbleSort<T>(T[] list) where T : IComparable
{
    bool madeChanges;
    int itemCount = list.Length;
    do
    {
        madeChanges = false;
        itemCount--;
        for (int i = 0; i < itemCount; i++)
        {
            if (list[i].CompareTo(list[i + 1]) > 0)
            {
                (list[i], list[i + 1]) = (list[i + 1], list[i]);
                madeChanges = true;
            }
        }
    } while (madeChanges);
}
#endregion

#region Helper methods
static void Print<T>(T[] list) => Console.WriteLine(JsonSerializer.Serialize(list));
//static void Print<T>(MySpan<T> list) => Print(list.ToArray());
#endregion
```

## The Problem

Let's sort some numbers in-place:

```csharp
// Sort some numbers in-place
var numbers = new[] { 5, 4, 3, 2, 1 };
BubbleSort(numbers);
Print(numbers);
```

New challenge: Ignore the last two numbers when sorting the array. The following solution produces a lot of "garbage".

```csharp
// Ignore the last two numbers when sorting the array.
// This solution produces a lot of "garbage".
numbers = new[] { 5, 4, 3, 2, 1 };
var numbersToSort = numbers[..^2];
BubbleSort(numbersToSort);
numbersToSort.CopyTo(numbers, 0);
Print(numbers);
```

## A Struct for a Slice of an Array

Wouldn't it be nice if we had a struct that represents a slice of an existing array without having to copy it? 

```csharp
/// <summary>
/// Naive implementation of a range over an array
/// </summary>
/// <remarks>
/// Use this struct to illustrate the idea behind C#'s Span. This is NOT
/// production code. You should ALWAYS prefer C#'s Span class.
/// </remarks>
public readonly ref struct MySpan<T>
{
    private readonly T[] underlyingArray;
    private readonly int start;
    private readonly int length;

    public MySpan(T[] source) : this(source, 0, source.Length) { }

    public MySpan(T[] source, int start, int length) =>
        (underlyingArray, this.start, this.length) = (source, start, length);

    public static implicit operator MySpan<T>(T[] source) => new MySpan<T>(source);

    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(underlyingArray, start, result, 0, Length);
        return result;
    }

    public int Length => length;

    public ref T this[int index] => ref underlyingArray[start + index];

    public ref T this[Index index] => ref underlyingArray[start + index.GetOffset(Length)];

    public MySpan<T> this[Range range]
    {
        get
        {
            var startFromBegin = range.Start.GetOffset(underlyingArray.Length);
            var endFromBegin = range.End.GetOffset(underlyingArray.Length);
            return new MySpan<T>(underlyingArray, 
                startFromBegin,
                endFromBegin - startFromBegin);
        }
    }
}
```

This enables the following implementation of our previous changes. It does not allocate unnecessary memory. BTW - in order to run this code sample, you have to copy `BubbleSort` and change `T[]` in the method signature to `MySpan<T>`.

```csharp
// Let's try the same with our span
MySpan<int> numbersSpan = new[] { 5, 4, 3, 2, 1 };
BubbleSort(numbersSpan[..^2]);
Print(numbersSpan);
```

## Enter `Span<T>`

Luckily, we do not need to implement `MySpan` because C# has a much better implementation: [`Span<T>`](https://source.dot.net/#System.Private.CoreLib/Span.cs,32). So we can replace `MySpan` with `Span`:

```csharp
// Let's try the same with our span
Span<int> numbersSpan = new[] { 5, 4, 3, 2, 1 };
BubbleSort(numbersSpan[..^2]);
Print(numbersSpan);
```

## Stack, Heap, Unmanaged Heap

Take a look at [implementation of `Span<T>`](https://source.dot.net/#System.Private.CoreLib/Span.cs,32). As you can see, its implementation is not limited to arrays. It works with pointers. Therefore, it can handle data on the stack, heap, and unmanaged heap.

> `Span<T>` is a unified programming model over different types of memory.

Microsoft is gradually supporting `Span<T>` in more and more API in their base class library (BCL). As a result, you do not need to copy memory in order to use BCL functions.

The following code demonstrates how our bubble sort can be applied to different kinds of memory.

```csharp
// C#'s Span<T> can not only handle array on the heap, but also
// arrays on the stack and in unmanaged memory.
Span<int> numbersSpan = new[] { 5, 4, 3, 2, 1 };

//Span<int> numbersSpan = stackalloc int[] { 5, 4, 3, 2, 1 };

//numbers = new[] { 5, 4, 3, 2, 1 };
//Span<int> numbersSpan;
//IntPtr ptr = Marshal.AllocHGlobal(numbers.Length * sizeof(int));
//try
//{
//    unsafe { numbersSpan = new Span<int>((int*)ptr, numbers.Length); }
//    numbers.CopyTo(numbersSpan);
//}
//finally
//{
//    Marshal.FreeHGlobal(ptr);
//}

BubbleSort(numbersSpan[..^2]);
Print(numbersSpan);
```

## Some Features and Limitations

We can iterate over elements of a span.

```csharp
foreach (var number in numbersSpan)
{
    Console.WriteLine(number);
}
```

But we cannot use Linq.

```csharp
// numbersSpan.Where(n => n % 2 == 0) -> results in an error
```

Spans work with strings.

```csharp
ReadOnlySpan<char> letters = "20,22".AsSpan();
var number1 = int.Parse(letters[..2]);
var number2 = int.Parse(letters[^2..]);
Console.WriteLine(number1 + number2);
```

But spans don't work with lists.

```csharp
// numbersSpan = new List<int> { 5, 4, 3, 2, 1 }; -> results in an error
```

We cannot return a stackalloc'ed span.

```csharp
static Span<int> ReturnSomeNumbers() => /*stackalloc*/ new int[] { 5, 4, 3, 2, 1 };
numbersSpan = ReturnSomeNumbers();
BubbleSort(numbersSpan[..^2]);
Print(numbersSpan);
```

Span is a ref struct. Therefore, it must be on the stack and can never live on the heap.

```csharp
// object o = new Span<int>(new[] { 5, 4, 3, 2, 1 }); -> results in an error
Func<int> first = () =>
{
    Span<int> innerNumbersSpan = new[] { 5, 4, 3, 2, 1 };
    // return new Func<int>(() => innerNumbersSpan[0]); -> results in an error
    return 42;
};
//var listOfSpans = new List<Span<int>>(42); -> results in an error
```

## Building Strings with Spans

Strings are immutable? Well, not always. With `String.Create` you have the chance to get a mutable version of the string at construction time. Note that the following sample is simplified because it can only handle single-digit numbers. Your real-world algorithms will be more complicated.

```csharp
var csvNumbers = string.Create(numbers.Length - 1 + numbers.Length, numbers, (buffer, source) =>
{
    for (var i = 0; i < source.Length; i++)
    {
        if (i > 0)
        {
            buffer[0] = ',';
            buffer = buffer[1..];
        }

        source[i].TryFormat(buffer, out var charsWritten);
        buffer = buffer[charsWritten..];
    }
});
Console.WriteLine(csvNumbers + "!");
```

Are things like that done in real life? Yes, see [`Range.ToString`](https://source.dot.net/#System.Private.CoreLib/Range.cs,54) for instance.

## Are They Fast?

Yes!

See [Benchmark](https://github.com/rstropek/Samples/blob/33e5b341988f3c5dc38e107ff34cd33893a7ccc3/CSharp7/Span/StringsWithSpan.cs#L122).

## `Memory<T>`

What should you do if the limitations of Span are a problem for your use case (e.g. you need to use it with lambdas, in classes, as type parameters)? `Memory<T>` is your friend in such cases.

```csharp
var manyNumbers = new List<Memory<int>>
{
    new [] { 5, 4, 3, 2, 1 },
    new [] { 9, 8, 7, 6, 5 }
};
manyNumbers.ForEach(numbersMemory =>
{
    // Get a Span from the Memory for local processing
    var localSpan = numbersMemory.Span;
    BubbleSort(localSpan);
    Print(localSpan);
});
```

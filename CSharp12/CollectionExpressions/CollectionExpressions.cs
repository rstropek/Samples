using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using Xunit;

namespace CollectionExpressions;

// ==================================================================================================
// READ MORE ABOUT COLLECTION EXPRESSIONS AT
// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#collection-expressions
// ==================================================================================================

public class CollectionExpressions
{
    record Hero(string Name, string Power);

    [Fact]
    public void Arrays()
    {
        int[] numbers = [1, 2, 3, 4, 5];
        // var is not allowed for collection expressions. You must specify a type.
        // var numbers2 = [1, 2, 3, 4, 5];

        Hero[] heroes = [new Hero("Batman", "Money"), new Hero("Superman", "Strength")];

        // New to jagged arrays? https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays
        int[][] jaggedArray = [[1, 2, 3], [4, 5, 6], numbers];
        numbers[0] = 10;
        Assert.Equal(numbers[0], jaggedArray[2][0]);

        // It is not possible to initialize a two-dimensional array with collection expressions.
        // int[,] matrix = [[1, 2, 3], [4, 5, 6]];
    }

    [Fact]
    public void OtherCollections()
    {
        // Collection expression can be used with types that support collection initializers
        List<int> numbers = [1, 2, 3, 4, 5];
        HashSet<int> uniqueNumbers = [1, 2, 3, 4, 5];
        ConcurrentBag<int> concurrentNumbers = [1, 2, 3, 4, 5];

        // The following lines will result in a List<int>
        IEnumerable<int> numberEnumerable = [1, 2, 3, 4, 5];
        IList<int> numberList = [1, 2, 3, 4, 5];
        numberList.Add(7);
        Assert.Equal(6, numberList.Count);

        // It is not possible to initialize a dictionary with collection expressions.
        // We have to use the "vintage" way ;-)
        Dictionary<string, int> numberMap;
        numberMap = new() { ["one"] = 1, ["two"] = 2, ["three"] = 3 };
        numberMap = new() { { "one", 1 }, { "two", 2 }, { "three", 3 } };

        // However, we can initialize the dictionary with an empty collection expression.
        numberMap = [];
    }

    [Fact]
    public void ExpressionBodiedMembers()
    {
        // Expression-bodied members can return collection expressions
        static int[] GetNumbers() => [1, 2, 3, 4, 5];
        Assert.Equivalent((int[])[1, 2, 3, 4, 5], GetNumbers(), true);
    }

    [Fact]
    public void MethodArguments()
    {
        static int GetSum(IEnumerable<int> numbers) => numbers.Sum();
        // Note that you can use collection expressions as method arguments
        // (in this case, a list is created).
        Assert.Equal(15, GetSum([1, 2, 3, 4, 5]));
    }

    [Fact]
    public void Spread()
    {
        int[] numbers = [1, 2, 3, 4, 5];
        int[] moreNumbers = [6, 7, 8, 9, 10];
        int[] allNumbers = [..numbers, ..moreNumbers];
        Assert.Equal(10, allNumbers.Length);

        // Note that the spread operator COPIES the elements from the source array(s).
        // Use dnSpy to see the generated code.
        numbers[0] = 10;
        Assert.Equal(1, allNumbers[0]);
    }

    [Fact]
    public void Spans()
    {
        Span<int> numbers = [1, 2, 3, 4, 5];

        ReadOnlySpan<int> moreNumbers = [6, 7, 8, 9, 10];
        ReadOnlySpan<char> hello = ['h', 'e', 'l', 'l', 'o'];
        ReadOnlySpan<int> allNumbers = [..numbers, ..moreNumbers];
    }

    [Fact]
    public void CustomClass()
    {
        // Collection expression is supported here because of CollectionBuilderAttribute.
        RunningSumBuffer<int> numbers = [1, 2, 3, 4, 5];
        // Note how we are using collection expression with type casting here.
        Assert.Equivalent((int[])[1, 3, 6, 10, 15], numbers, true);
    }
}

// New to INumber & Co (Generic Math)? Read more at
// https://learn.microsoft.com/en-us/dotnet/standard/generics/math

[CollectionBuilder(typeof(RunningSumBuilder), "Create")]
public class RunningSumBuffer<T>(IEnumerable<T> values) : IEnumerable<T> where T: INumber<T>
{
    private readonly List<T> sums = new(values);

    public IEnumerator<T> GetEnumerator() => sums.AsEnumerable<T>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => sums.GetEnumerator();
}

internal static class RunningSumBuilder
{
    public static RunningSumBuffer<T> Create<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        var numbers = new List<T>(values.Length);
        var sum = T.Zero;
        foreach (var value in values)
        {
            sum += value;
            numbers.Add(sum);
        }

        return new RunningSumBuffer<T>(numbers);
    }
}
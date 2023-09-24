using System.Collections.Immutable;

namespace Lists;

public class ImmutableCollections
{
    [Fact]
    public void ImmutableArrayBasics()
    {
        // We start with an empty array.
        var array = ImmutableArray<int>.Empty;

        // We CAN change the array using Add, Remove, Insert, etc.
        // However, NOTE re-assignment here. Original arrays
        // stays the same.
        array = array.Add(1);

        var array2 = array.Add(2).Add(3).Add(4).Remove(4);
        Assert.Single(array);
        Assert.Equal(3, array2.Length);

        // NOTE: As the original array is never changed, immutable
        // collections are always thread-safe. You can even iterate over
        // a snapshot of the immutable collection while creating
        // new variants of the collection.
    }

    [Fact]
    public void ImmutableArrayCreate()
    {
        // Create immutable array from existing collection
        var array1 = ImmutableArray.Create(1, 2, 3, 4, 5);
        var array2 = ImmutableArray.Create([1, 2, 3, 4, 5]);

        // Use builder pattern
        var builder = ImmutableArray.CreateBuilder<int>();
        builder.Add(1);
        builder.AddRange(new int[] { 2, 3, 4, 5 });
        var array3 = builder.ToImmutable();

        Assert.Equal(5, array1.Length);
        Assert.Equal(5, array2.Length);
        Assert.Equal(5, array3.Length);

        // We can also create a builder from an immutable array.
        var builder2 = array3.ToBuilder();
        builder2.Add(6);
        var array4 = builder2.ToImmutable();
        Assert.Equal(6, array4.Length);
    }

    // NOTE that we do not add examples for
    // - ImmutableList
    // - ImmutableDictionary
    // - ImmutableHashSet
    // - ImmutableSortedDictionary
    // - ImmutableSortedSet
    // - ImmutableQueue
    // - ImmutableStack
    // here as they are very similar to their mutable counterparts. However,
    // NOTE that ImmutableList is a linked list, not (like List<T>) an array list.
}
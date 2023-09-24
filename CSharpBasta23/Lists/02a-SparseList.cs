namespace Lists;

// NOTE that this code is used to demonstrate the possibilities
// of Collection<T>. Our goal is not to implement a production-ready
// sparse list.

/// <summary>
/// Represents a sparse list where elements are stored in a dictionary with their index as the key.
/// </summary>
/// <typeparam name="T">Specifies the element type of the list.</typeparam>
/// <remarks>
/// <para>This code is not production-ready. It is for demonstration purposes only.
/// Benchmarks have not been done. Error handling is not implemented.</para>
/// <para>Note that this class is not thread-safe.</para>
/// <remarks>
public class SparseList<T> : IList<T>
{
    private readonly SortedDictionary<int, T> items = [];

    public int Count => items.Keys.Count == 0 ? 0 : items.Keys.Max() + 1;

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => items.TryGetValue(index, out var value) ? value! : default!;
        set => items[index] = value;
    }

    public void Add(T item) => items[Count] = item;

    public void Clear() => items.Clear();

    public bool Contains(T item) => items.ContainsValue(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = items.TryGetValue(i, out var value) ? value! : default!;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return items.TryGetValue(i, out var value) ? value! : default!;
        }
    }

    public int IndexOf(T item)
    {
        foreach (var entry in items)
        {
            if (EqualityComparer<T>.Default.Equals(entry.Value, item))
            {
                return entry.Key;
            }
        }

        return -1;
    }

    public void Insert(int index, T item) => items[index] = item;

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            items.Remove(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index) => items.Remove(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class SparseListTests
{
    [Fact]
    public void Count_Zero_EmptyList()
    {
        var list = new SparseList<int>();
        Assert.Empty(list);
    }

    [Fact]
    public void Count_ReturnsCorrectly()
    {
        var list = new SparseList<int> { [9_999_999] = 1 };
        Assert.Equal(10_000_000, list.Count);
    }

    [Fact]
    public void Add_AddsItems()
    {
        var list = new SparseList<int>() { 1, 2 };
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Contains_ReturnsCorrectly()
    {
        var list = new SparseList<int> { 1, 2 };
        list[10_000_000] = 3;
        Assert.Contains(1, list);
        Assert.Contains(2, list);
        Assert.Contains(3, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void Indexer_ReturnsDefaultForUnsetItems()
    {
        var list = new SparseList<int> { [0] = 1, [10_000_000] = 2 };
        Assert.Equal(0, list[1_000_000]);
    }

    [Fact]
    public void Indexer_SetsAndGetsItems()
    {
        var list = new SparseList<int> { 1 };
        list[10_000_000] = 3;
        Assert.Equal(3, list[10_000_000]);
    }

    [Fact]
    public void Enumerator_YieldsAllItemsIncludingDefault()
    {
        var list = new SparseList<int> { 1 };
        list[2] = 3;

        var enumerated = list.ToList();
        Assert.Equal(3, enumerated.Count);
        Assert.Equal(1, enumerated[0]);
        Assert.Equal(0, enumerated[1]);
        Assert.Equal(3, enumerated[2]);
    }

    [Fact]
    public void CopyTo_CopiesAllItemsIncludingDefault()
    {
        var list = new SparseList<int> { 1 };
        list[2] = 3;

        var array = new int[3];
        list.CopyTo(array, 0);

        Assert.Equal(1, array[0]);
        Assert.Equal(0, array[1]);
        Assert.Equal(3, array[2]);
    }

    [Fact]
    public void IndexOf_ReturnsIndex()
    {
        var list = new SparseList<int> { 1, 2 };
        Assert.Equal(1, list.IndexOf(2));
    }

    [Fact]
    public void Remove_RemovesItem()
    {
        var list = new SparseList<int> { 1, 2 };
        Assert.True(list.Remove(2));
        Assert.DoesNotContain(2, list);
        Assert.Single(list);
    }

    [Fact]
    public void Insert_InsertsItemAtGivenIndex()
    {
        var list = new SparseList<int> { 1, 2 };
        list.Insert(10_000_000, 5);
        Assert.Equal(5, list[10_000_000]);
    }

    [Fact]
    public void Clear_ClearsAllItems()
    {
        var list = new SparseList<int> { 1, 2 };
        list.Clear();
        Assert.Empty(list);
    }
}

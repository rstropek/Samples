namespace Lists;

public class ObjectModel
{
    [Fact]
    public void Collection()
    {
        // Create a collection. It uses a List<T> internally.
        Collection<int> numbers = [1, 2, 3, 4, 5];

        // We can access the elements of a collection using an index.
        Assert.Equal(1, numbers[0]);

        // We can manipulate the content of a collection (see also Insert,
        // Remove, RemoveAt, etc.).
        numbers.Add(6);

        // NOTE that Collection does NOT offer methods like sort and
        // binary search. Here it differes from List<T>.
    }

    private enum Operation { Insert, Remove, Set };
    private class CollectionWithHistory<T> : Collection<T>
    {
        public List<(Operation Op, int index)> Operations = [];

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Operations.Add((Operation.Insert, index));
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Operations.Add((Operation.Remove, index));
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            Operations.Add((Operation.Set, index));
        }
    }

    [Fact]
    public void CollectionOverrides()
    {
        // In contrast to List<T>, Collection<T> offers virtual methods
        // that you can override to get notified about changes.

        CollectionWithHistory<int> numbers = [1, 2, 3];
        Assert.Equal(3, numbers.Operations.Count);
        Assert.All(numbers.Operations, op => Assert.Equal(Operation.Insert, op.Op));

        numbers[1] = 4;
        Assert.Equal(4, numbers.Operations.Count);
        Assert.Equal(Operation.Set, numbers.Operations[3].Op);

        numbers.RemoveAt(1);
        Assert.Equal(5, numbers.Operations.Count);
        Assert.Equal(Operation.Remove, numbers.Operations[4].Op);

        // NOTE that this mechanism is used by ObservableCollection<T> to
        // notify about changes via events.
    }

    [Fact]
    public void CollectionWrapper()
    {
        // NOTE that collections can be wrappers around other collections.
        // Therefore, it is a good idea to return Collections instead
        // of Lists in public APIs. With this, you can change the internal
        // implementation without breaking the API.

        var numbers = new Collection<int>(new SparseList<int>()
        {
            [10_000_000] = 1
        });

        Assert.Equal(1, numbers[10_000_000]);
        Assert.Equal(10_000_001, numbers.Count);
    }

    // NOTE that similar to Collection<T> there is KeyedCollection<TKey, TItem>
    // that allows to access items using a key. KeyedCollection<TKey, TItem> is 
    // to Dictionary<TKey, TValue> what Collection<T> is to List<T>.
}
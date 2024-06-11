namespace Lists;

public class GenericCollections
{
    [Fact]
    public void GoodOldLists()
    {
        List<int> numbers = [1, 3, 2, 4, 5];

        // List is an array list -> we have a capacity property that we
        // can read and write (see also TrimExcess, EnsureCapacity, etc.).
        Assert.True(numbers.Capacity >= numbers.Count);

        // We can access the elements of a list using an index.
        Assert.Equal(1, numbers[0]);

        // We can manipulate the content of a list (see also Insert,
        // Remove, RemoveAt, etc.).
        numbers.Add(6);
        Assert.Equal(6, numbers.Count);

        // We can sort a list in-place and can use sorted lists
        // for binary search.
        numbers.Sort();
        Assert.Equal(2, numbers[1]);
        Assert.Equal(1, numbers.BinarySearch(2));
    }

    [Fact]
    public void ReadOnlyCollection()
    {
        List<int> numbers = [1, 3, 2, 4, 5];

        // We can create a read-only collection from a list (wrapper).
        // NOTE that this is in System.Collection.ObjectModel.
        ReadOnlyCollection<int> readOnlyNumbers = numbers.AsReadOnly();

        // We can access the elements of a read-only collection using an index.
        Assert.Equal(1, readOnlyNumbers[0]);

        // NOTE that changes in the underlying collection are visible
        // through the read-only collection.
        numbers.Add(6);
        Assert.Equal(6, readOnlyNumbers.Count);

        // NOTE that something like this exists for Dictionaries, too
        // (ReadOnlyDictionary<TKey, TValue>).
    }

    [Fact]
    public void Dictionary()
    {
        // Dictionaries are hash maps mapping a key to a value.
        Dictionary<string, int> numbers = new()
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };

        // We can access the elements of a dictionary using a key.
        // Access is very fast (nearly O(1)).
        Assert.Equal(1, numbers["one"]);

        // Similar to list, a dictionary has a capacity that we can control.
        numbers.EnsureCapacity(100);
        numbers.TrimExcess();

        // NOTE that dictionaries use the GetHashCode method of the key. If
        // GetHashCode is broken, the dictionary will not work correctly.
        Dictionary<WrongStruct, int> stupidDictionary = [];
        for (var i = 0; i < 100; i++) { stupidDictionary[new(i)] = i; }
        try
        {
            for (var i = 0; i < 100; i++) { Assert.Equal(i, stupidDictionary[new(i)]); }
            Assert.Fail("This line will never be reached because of the wrong GetHashCode implementation.");
        }
        catch (Exception) { }
    }

    // ATTENTION: This struct implementation is WRONG. It is only used to demonstrate
    // the importance of a good GetHashCode implementation.
    private record struct WrongStruct(int Content)
    {
        // NOTE that this implementation of GetHashCode is wrong. It MUST NOT
        // return values that change. The hash code must be stable over the
        // lifetime of the object.
        public override readonly int GetHashCode() => Random.Shared.Next();
    }

    [Fact]
    public void HashSet()
    {
        // Hash sets are sets of unique elements. They are somwhat like a
        // dictionary without values.
        HashSet<int> numbers = [3, 1, 5, 4, 2, 5,];
        Assert.Equal(5, numbers.Count);

        // We can check if a hash set contains an element (very fast).
        Assert.Contains(1, numbers);
        Assert.True(numbers.TryGetValue(2, out var two));
        Assert.Equal(2, two);

        // We can use efficient set operations (see also IntersectWith, ExceptWith, etc.).
        HashSet<int> otherNumbers = [4, 5, 6, 7, 8];
        var expected = new HashSet<int>([1, 2, 3, 4, 5, 6, 7, 8]);
        var union = numbers.Union(otherNumbers);
        // Note that we cannot simply compare the sets, because the order
        // of the elements is not defined in a hash set. Note that there is
        // a variant (SortedSet<T>) that maintains the elements in sorted order
        // (uses IComparer<T>).
        Assert.Equal(expected.Count, union.Count());
        foreach (var n in expected) { Assert.Contains(n, union); }

        // Similar to list, hash sets have a capacity that we can control.
        numbers.EnsureCapacity(100);
        numbers.TrimExcess();
    }

    [Fact]
    public void LinkedList()
    {
        // Linked lists are double-linked lists where each element has a reference to the
        // next and the previous element. This allows for fast insertions and deletions.
        LinkedList<int> numbers = new();

        // We can add elements anywhere in the list.
        numbers.AddFirst(1);
        numbers.AddLast(4);
        var three = numbers.AddAfter(numbers.First!, 3);
        var two = numbers.AddBefore(three, 2);
        Assert.Equal(1, numbers.First!.Value);
        Assert.Equal(4, numbers.Last!.Value);
        Assert.Equal(2, two.Value);

        // We can remove a list node and insert it somewhere else. In this case, no
        // additional heap allocations are required.
        numbers.Remove(two);
        numbers.AddFirst(two);
        Assert.Equal(new int[] { 2, 1, 3, 4 }, numbers);

        // PREFER arrays or List<T> for their simplicity, better memory usage, and 
        // fast indexed access when the collection size is known and does not change, 
        // or changes infrequently.
        //
        // CONSIDER LinkedList<T> when you need to frequently and efficiently add or 
        // remove elements in the middle of the collection, and do not require fast 
        // indexed access.
        //
        // When in doubt, PREFER List<T> over LinkedList<T>.
        //
        // When performance really matters, DO run benchmarks with your specific
        // data and algorithms (BenchmarkDotNet is a good choice).
    }

    [Fact]
    public void QueueAndPriorityQueue()
    {
        // Queue is a First-In-First-Out (FIFO) collection, elements are enqueued to the back
        // and dequeued from the front.
        Queue<int> numbers = new();
        numbers.Enqueue(1);
        numbers.Enqueue(2);
        numbers.Enqueue(3);

        Assert.Equal(1, numbers.Dequeue());
        Assert.Equal(2, numbers.Peek());
        Assert.Equal(2, numbers.Count);

        // PriorityQueue is a collection where each element has an associated priority.
        // Elements are dequeued based on their priority. Elements with the lowest priority 
        // are dequeued first. If multiple elements have the same priority, 
        // they are dequeued based on their order in the priority queue.
        PriorityQueue<int, int> priorityQueue = new();
        priorityQueue.Enqueue(element: 1, priority: 3);
        priorityQueue.Enqueue(2, 1);
        priorityQueue.Enqueue(3, 2);

        // Dequeues the element with the lowest priority.
        Assert.True(priorityQueue.TryDequeue(out var item, out var prio));
        Assert.Equal(2, item);
        Assert.Equal(1, prio);
        Assert.Equal(2, priorityQueue.Count);

        // Similar to list, queues have a capacity that we can control.
        numbers.EnsureCapacity(100);
        numbers.TrimExcess();
    }

    [Fact]
    public void SortedCollections()
    {
        // SortedList is a list that maintains its elements in ascending order or the keys.
        SortedList<int, string> sortedList = new() { { 3, "Three" }, { 1, "One" }, { 2, "Two" } };

        // Assert elements are in ascending order. NOTE how we can access the elements
        // using an index. This is not possible with a SortedDictionary.
        Assert.Equal("One", sortedList.Values[0]);
        Assert.Equal("Two", sortedList.Values[1]);
        Assert.Equal("Three", sortedList.Values[2]);

        // Assert contains key.
        Assert.True(sortedList.ContainsKey(1));

        // SortedDictionary is a dictionary that maintains its elements in ascending order.
        SortedDictionary<int, string> sortedDictionary = new() { [3] = "Three", [1] = "One", [2] = "Two" };

        // Assert contains key (fast because of binary search tree).
        Assert.True(sortedDictionary.ContainsKey(1));

        // Assert elements are in ascending order.
        Assert.Equal("One", sortedDictionary.Values.First());
        Assert.Equal("Two", sortedDictionary.Values.Skip(1).First());
        Assert.Equal("Three", sortedDictionary.Values.Last());

        // PREFER SortedList if you need to access elements by index, and the 
        // list size doesn't change often.
        //
        // PREFER SortedDictionary if you have frequent changes in size and 
        // don’t need to access elements by index.
    }

    [Fact]
    public void Stack()
    {
        // Stack is a Last-In-First-Out (LIFO) collection.
        // Elements are pushed onto the stack and popped off the stack.
        Stack<int> numbers = new();

        // Pushing elements onto the stack
        numbers.Push(1);
        numbers.Push(2);
        numbers.Push(3);
        Assert.Equal(3, numbers.Count);

        Assert.Equal(3, numbers.Peek());
        Assert.Equal(3, numbers.Pop());
        Assert.Equal(2, numbers.Count);

        // Similar to list, queues have a capacity that we can control.
        numbers.EnsureCapacity(100);
        numbers.TrimExcess();
    }
}
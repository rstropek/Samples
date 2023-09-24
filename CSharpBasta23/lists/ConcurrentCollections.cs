namespace Lists;

public class ConcurrentCollections
{
    [Fact]
    public async Task ListIsNotThreadSafe()
    {
        try
        {
            var list = new List<int>();
            var tasks = new List<Task>();

            // Start 10 tasks, each of which adds 1000 items to the list.
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 10000; j++)
                    {
                        list.Add(j);
                    }
                }));
            }

            // Wait for all tasks to complete.
            await Task.WhenAll(tasks);

            // The list should have 10,000 items, right? Well, very likely not.
            // This is because List is not thread-safe.
            Assert.Equal(100 * 10000, list.Count);

            Assert.Fail("This statement will very likely not be reached");
        }
        catch (Exception)
        {
        }

        // NOTE that collections in System.Collections.Generic are NOT
        // thread-safe. If you need thread-safety, use the collections
        // in System.Collections.Concurrent.
    }

    [Fact]
    public void ConcurrentBag()
    {
        // ConcurrentBag is a thread-safe, un-ordered collection.
        // It is similar to List, but it does not guarantee the order
        // of the elements.
        ConcurrentBag<int> bag = [1, 2, 3];

        // You can add elements to the bag. Duplicates are supported.
        bag.Add(3);
        Assert.Equal(4, bag.Count);
        
        // We can peek into the bag or take an element from it.
        Assert.True(bag.TryPeek(out _));
        Assert.True(bag.TryTake(out _));
        Assert.Equal(3, bag.Count);

        // Note that the enumerator will be a SNAPSHOT of the bag.
        // Concurrent changes to the bag will not be reflected in the
        // enumerator.
        foreach(var i in bag)
        {
            Assert.True(i < 5);
        }
    }

    [Fact]
    public void ProducerConsumer()
    {
        var bag = new ConcurrentBag<int>();
        var done = false;
        var numberReceived = 0;

        void Produce()
        {
            // Add 10 items to the bag. Wait 2ms between each add.
            for (int i = 0; i < 10; i++)
            {
                bag.Add(i);
                Thread.Sleep(2);
            }

            // Signal that we are done. It is fine to use a simple
            // boolean here because we have a single producer.
            done = true;
        }

        void Consume()
        {
            // Keep taking items from the bag until the producer is done.
            while (!done)
            {
                while (bag.TryTake(out var i))
                {
                    numberReceived++;
                }

                // Currently, the bag is empty. Wait 2ms before trying again.
                Thread.Sleep(2);
            }

        }

        var producer = Task.Run(Produce);
        var consumer1 = Task.Run(Consume);
        var consumer2 = Task.Run(Consume);

        Task.WaitAll(producer, consumer1, consumer2);

        // We should have received 10 items.
        Assert.Equal(10, numberReceived);
    }

    // NOTE that there are concurrent versions of
    // - Dictionary (ConcurrentDictionary<TKey, TValue>)
    // - Queue (ConcurrentQueue<T>)
    // - Stack (ConcurrentStack<T>)

    [Fact]
    public void BlockingCollection()
    {
        // BlockingCollection is a wrapper around a concurrent collection
        // that blocks the consumer when the collection is empty.
        // It also blocks the producer if a specified upper-bound
        // is reached.
        //
        // By default, BlockingCollection uses ConcurrentQueue<T>.
        // However, you can specify a different collection type.
        var collection = new BlockingCollection<int>(boundedCapacity: 3);
        var numberReceived = 0;

        void Produce()
        {
            // Add 10 items to the bag. Wait 2ms between each add.
            for (int i = 0; i < 10; i++)
            {
                // Add will be blocked if capacity is reached.
                collection.Add(i);
                Thread.Sleep(2);
            }

            collection.CompleteAdding();
        }

        void Consume()
        {
            // Keep taking items from the bag until the producer is done.
            // foreach loop will be blocked if the collection is empty.
            foreach(var i in collection.GetConsumingEnumerable())
            {
                numberReceived++;
            }

        }

        var producer = Task.Run(Produce);
        var consumer1 = Task.Run(Consume);
        var consumer2 = Task.Run(Consume);

        Task.WaitAll(producer, consumer1, consumer2);

        // We should have received 10 items.
        Assert.Equal(10, numberReceived);
    }
}
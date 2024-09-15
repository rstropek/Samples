var collection = new StampCollection();
collection.AddStamp("Penny Black");
collection.AddStampManually("Blue Mauritius");

var newCollection = new NewStampCollection();
newCollection.AddStamp("Baden 9 Kreuzer");
newCollection.AddStampManually("Inverted Jenny");
if (newCollection.TryAddStamp("The Penny Red"))
{
    Console.WriteLine("Stamp added");
}

class StampCollection
{
    // A list to hold the collection of stamps.
    private List<string> Collection { get; } = [];

    // A dedicated object used for locking to ensure thread safety.
    private readonly object lockObject = new();

    // Method to add a stamp to the collection using the 'lock' keyword.
    public void AddStamp(string stamp)
    {
        // The lock keyword in C# is a shorthand for acquiring and releasing a lock on a specified object. 
        // It ensures that only one thread can execute the code block at a time, providing thread safety. 
        // When a thread enters a lock block, it acquires an exclusive lock on the specified object
        // (lockObject in this case). The lock is automatically released when the thread exits the block, 
        // even if an exception occurs.
        //
        // Using a dedicated lock object (lockObject) is a common practice because it avoids potential 
        // deadlocks and ensures that the lock is only used for synchronization purposes. Locking on this 
        // or other publicly accessible objects can lead to complex and hard-to-debug issues, as other 
        // code might inadvertently lock on the same object.
        lock (lockObject)
        {
            Collection.Add(stamp);
        }
    }

    // Method to add a stamp to the collection manually using Monitor.Enter and Monitor.Exit.
    public void AddStampManually(string stamp)
    {
        bool lockTaken = false;
        try
        {
            // Monitor.Enter is used to acquire an exclusive lock on the specified object (lockObject).
            // The 'ref lockTaken' parameter indicates whether the lock was successfully taken.
            Monitor.Enter(lockObject, ref lockTaken);
            Collection.Add(stamp);
        }
        finally
        {
            // If the lock was successfully taken, Monitor.Exit is called to release the lock.
            if (lockTaken)
            {
                Monitor.Exit(lockObject);
            }
        }
    }
}

class NewStampCollection
{
    private List<string> Collection { get; } = [];

    // Here we use the new Lock lock object of .NET 9.
    private readonly Lock lockObject = new();

    public void AddStamp(string stamp)
    {
        // The lock keyword understands the new lock object.
        lock (lockObject)
        {
            Collection.Add(stamp);
        }

		// Will lead to:
        //using(lockObject.EnterScope())
        //{
        //    Collection.Add(stamp);
        //}
    }

    public void AddStampManually(string stamp)
    {
        lockObject.Enter(); // Will wait until the lock is available.
                            // In a Windows STA thread (e.g. a GUI thread), other code in the same thread can still run.
        try
        {
            Collection.Add(stamp);
        }
        finally
        {
            lockObject.Exit();
        }
    }

    public bool TryAddStamp(string stamp)
    {
        if (lockObject.TryEnter())
        {
            try
            {
                Collection.Add(stamp);
                return true;
            }
            finally
            {
                lockObject.Exit();
            }
        }

        return false;
    }
}

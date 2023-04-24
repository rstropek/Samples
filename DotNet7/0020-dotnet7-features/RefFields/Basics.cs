static class Basics
{
    public static void Demo()
    {
        var v = 42;
        var c = new Container<int>(ref v);
        Console.WriteLine(c.Value);

        // Change v. As c has a reference to v, c.Value changes, too.
        v = 43;
        Console.WriteLine(c.Value);

        // Change c.Value. As it is a referenct to v, v changes, too.
        c.Value = 44;
        Console.WriteLine(v);

        // Set c to default. That will lead to a null reference.
        c = default;
        try
        {
            c.Value = 0815;
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("InvalidOperationException");
        }
    }
}

/// <summary>
/// Simple container using a ref field in a ref struct
/// </summary>
ref struct Container<T>
{
    ref T value;

    public Container(ref T value)
    {
        this.value = ref value;
    }

    public T Value
    {
        get
        {
            // Note: value could be null, this is how you check:
            if (System.Runtime.CompilerServices.Unsafe.IsNullRef(ref value))
            {
                throw new InvalidOperationException();
            }

            return value;
        }

        set
        {
            if (System.Runtime.CompilerServices.Unsafe.IsNullRef(ref this.value))
            {
                throw new InvalidOperationException();
            }

            this.value = value;
        }
    }
}

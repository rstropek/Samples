namespace Ref;

// Exercise: Implement a sliding window that calculates the average of the window
// without allocating memory on the heap. The window should be slidable.

// First solution: mutable ref struct
ref struct SlidingWindow
{
    /// <summary>
    /// The values that the window is sliding over.
    /// </summary>
    private Span<int> values;

    /// <summary>
    /// The current window.
    /// </summary>
    private Span<int> window;

    /// <summary>
    /// The start index of <see cref="window"/> in <see cref="values"/>.
    /// </summary>
    private int windowStartIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlidingWindow"/> struct.
    /// </summary>
    public SlidingWindow(Span<int> values, int windowSize)
    {
        // Note usage of helper method to throw exception if windowSize is greater than values.Length
        // (available since .NET 8)
        ArgumentOutOfRangeException.ThrowIfGreaterThan(windowSize, values.Length, nameof(windowSize));

        this.values = values;
        window = values[..windowSize];
        windowStartIndex = 0;
    }

    /// <summary>
    /// Calculates the average of the current window.
    /// </summary>
    /// <remarks>
    /// Note that method is readonly as it does not mutate the struct.
    /// </remarks>
    public readonly float Average()
    {
        // Calc average of window without stack allocation
        int sum = 0;
        foreach (var v in window) { sum += v; }
        return (float)sum / window.Length;
    }

    /// <summary>
    /// Slides the window by one element.
    /// </summary>
    /// <returns>
    /// True if the window was slided successfully; otherwise, false.
    /// </returns>
    public bool Slide()
    {
        // Try to make method readonly -> does not work

        if (windowStartIndex + window.Length >= values.Length) { return false; }
        window = values[++windowStartIndex..(windowStartIndex + window.Length)];
        return true;
    }

    /// <summary>
    /// Sets the values that the window is sliding over.
    /// </summary>
    public Span<int> Values
    {
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(window.Length, value.Length, nameof(value));
            values = value;
            window = values[..window.Length];
            windowStartIndex = 0;
        }
    }
}

// Second solution: readonly ref struct
readonly ref struct ReadonlySlidingWindow
{
    private readonly ReadOnlySpan<int> values;
    private readonly ReadOnlySpan<int> window;
    private readonly int windowStartIndex;

    public ReadonlySlidingWindow(ReadOnlySpan<int> values, int windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(windowSize, values.Length, nameof(windowSize));

        this.values = values;
        window = values[..windowSize];
        windowStartIndex = 0;
    }

    public readonly float Average()
    {
        // Calc average of window without stack allocation
        int sum = 0;
        foreach (var v in window) { sum += v; }
        return (float)sum / window.Length;
    }

    /// <summary>
    /// Tries to slide the window by one element.
    /// </summary>
    /// <param name="nextWindow">
    /// The next window if sliding was successful, otherwise the current window.
    /// </param>
    /// <returns>
    /// True if the window was slided successfully; otherwise, false.
    /// </returns>
    public readonly bool TrySlide(out ReadonlySlidingWindow nextWindow)
    {
        if (windowStartIndex + window.Length >= values.Length)
        { 
            nextWindow = this;
            return false;
        }

        nextWindow = new ReadonlySlidingWindow(values[(windowStartIndex + 1)..], window.Length);
        return true;
    }
}

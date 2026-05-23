static T GetElementAt<T>(IEnumerable<T> things, int index)
{
    if (things is IList<T> or IReadOnlyList<T>)
    {
        // Note that we can use nameof with unbound generic types now (starting in C# 14).
        // The result will be just "IList" or "IReadOnlyList" without the generic type parameters.
        Console.WriteLine($"This is a {nameof(IList<>)} or {nameof(IReadOnlyList<>)}, indexing will be efficient.");
    }

    return things.ElementAt(index);
}

var things = new List<string> { "apple", "banana", "cherry" };
var element = GetElementAt(things, 1); // Should print the message about efficient indexing
Console.WriteLine(element);

var otherThings = new HashSet<string> { "dog", "cat", "mouse" };
var otherElement = GetElementAt(otherThings, 2);
Console.WriteLine(otherElement);

var numbers = new List<int>() { 1, 2, 3, 5, 8 };

// List pattern
if (numbers is [1, 2, 3, 5, 8])
{
    Console.WriteLine("Fibonacci");
}

// Property pattern
if (numbers is [var first, 2, 3, 5, var last] && first == 1 && last == 8)
{
    Console.WriteLine("Very special Fibonacci");
}

// Slice pattern
Console.WriteLine(numbers switch
{
    [1, .., var sl, 8] => $"Starts with 1, ends with 8, and 2nd last number is {sl}",
    [1, .., var sl, > 8 or < 0] =>
        $"Starts with 1, ends with something > 8 or < 0, and 2nd last number is {sl}",
    [1, _, _, ..] => "Starts with 1 and is at least 3 long",
    [1, ..] => "Starts with 1 and is at least 1 long",
    _ => "WAT?"
});

var heroes = new List<Hero>
{
    new("Superman", int.MaxValue),
    new("The Tick", 10),
};

if (heroes is [{ MaxJumpDistance: > 1000 }, { MaxJumpDistance: < 100, Name: var snd }])
{
    Console.WriteLine($"First can fly, second ('{snd}') cannot jump very far");
}

class Hero
{
    public string Name;
    public int MaxJumpDistance;
    public Hero(string name, int maxJumpDistance)
        => (Name, MaxJumpDistance) = (name, maxJumpDistance);
}

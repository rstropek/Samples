var numbers = new[] { 1, 2, 3, 4, 5 };

Console.WriteLine($"{numbers
        .Where(n => n % 2 == 0)
        .Select(n => n * n)
        .Sum()}");

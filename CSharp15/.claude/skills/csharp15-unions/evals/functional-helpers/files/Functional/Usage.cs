namespace Functional;

public sealed record User(int Id, string Name);
public sealed record NotFound(int Id);

public static class UserDirectory
{
    private static readonly Dictionary<int, User> Users = new() { [1] = new(1, "Ada"), [2] = new(2, "Linus") };

    public static Either<User, NotFound> Find(int id) =>
        Users.TryGetValue(id, out var user) ? user : new NotFound(id);

    public static Result<User> Rename(int id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return Result<User>.Failure("Name must not be empty");
        return Find(id).Match(
            user => Result<User>.Success(user with { Name = newName }),
            notFound => Result<User>.Failure($"User {notFound.Id} not found"));
    }

    public static string Greeting(int id) => Find(id).Match(
        user => $"Hello {user.Name}",
        notFound => $"No user {notFound.Id}");
}

public static class CsvImport
{
    public static Option<int> ParseInt(ReadOnlySpan<char> text) =>
        int.TryParse(text, out var value) ? Option<int>.Some(value) : Option<int>.None;

    /// <summary>Sums the first column; blank or invalid cells are skipped. Hot path.</summary>
    public static long SumFirstColumn(string csv)
    {
        long sum = 0;
        foreach (var line in csv.AsSpan().EnumerateLines())
        {
            int comma = line.IndexOf(',');
            var cell = comma < 0 ? line : line[..comma];
            if (ParseInt(cell).TryGetValue(out var value)) sum += value;
        }
        return sum;
    }
}

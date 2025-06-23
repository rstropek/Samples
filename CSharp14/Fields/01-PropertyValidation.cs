using System.Text.RegularExpressions;

namespace Fields;

public partial class User
{
    public required string Email
    {
        get;
        set => field = IsValidEmail(value) 
            ? value 
            : throw new ArgumentException("Invalid email format");
    }

    public required int Age
    {
        get;
        set => field = value is >= 0 and <= 150 
            ? value 
            : throw new ArgumentOutOfRangeException(nameof(value));
    }


    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
    private static bool IsValidEmail(string? email) => !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);
}
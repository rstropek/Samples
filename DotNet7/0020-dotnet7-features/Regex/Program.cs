using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace RegexDemo;

static partial class Program
{
    static void Main()
    {
        // Note: We do not compile regex here as we run it only once.
        // Consider RegexOptions.Compiled for other cases. BUT .NET 7
        // knows more tricks... details will follow.
        var regex = new Regex(@"^\d{4}-\d{2}-\d{2}$");
        Console.WriteLine(regex.IsMatch("2020-01-01"));

        Console.WriteLine(IsDate("2020-01-01", @"^\d{4}-\d{2}-\d{2}$"));

        regex = DateRegex();
        Console.WriteLine(regex.IsMatch("2020-01-01"));

        // Some Regex methods now support passing in spans
        ReadOnlySpan<char> input = "2020-01-02".AsSpan();
        Console.WriteLine(regex.IsMatch(input));
    }

    // Note new StringSyntax attribute. Will lead to nice editor support
    // regarding Regex in VS.
    private static bool IsDate(string dateString,
        [StringSyntax(StringSyntaxAttribute.Regex)]
        string? expression = null)
    {
        var regex = new Regex(expression ?? @"^\d{4}-\d{2}-\d{2}$");
        return regex.IsMatch(dateString);
    }

    // Here we use the new Regex Generator. Take a look at the generated code.
    // Note that we use the new option "non backtracking" which leads to much
    // better performance IF we do not need backtracking.
    [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.NonBacktracking)]
    private static partial Regex DateRegex();
}
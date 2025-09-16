using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("LogicTests")]

namespace Logic;

public record BuildPasswordParameters(
    int MinimumPasswordLength,
    bool DoSimpleReplacements = false,
    int? Seed = null);

public static class PasswordGenerator
{
    // Character replacement lookup table with both lowercase and uppercase mappings
    private static readonly Dictionary<char, char> SimpleReplacements = new()
    {
        ['a'] = '@', ['A'] = '@',
        ['s'] = '$', ['S'] = '$',
        ['o'] = '0', ['O'] = '0',
        ['i'] = '1', ['I'] = '1',
        ['e'] = '3', ['E'] = '3',
        ['t'] = '7', ['T'] = '7'
    };
    
    // Cached longest word length for precise capacity calculation
    private static readonly int LongestWordLength = FrequentWordList.FrequentWords.Max(word => word.Length);

    public static string BuildPassword(BuildPasswordParameters parameters) => BuildPassword(parameters, FrequentWordList.FrequentWords);

    internal static string BuildPassword(BuildPasswordParameters parameters, IReadOnlyList<string> words)
    {
        var random = parameters.Seed.HasValue ? new Random(parameters.Seed.Value) : new Random();

        // Calculate exact maximum capacity needed: MinimumPasswordLength + (longest word - 1)
        var maxCapacity = parameters.MinimumPasswordLength + LongestWordLength - 1;
        var passwordBuilder = new StringBuilder(maxCapacity);

        while (passwordBuilder.Length < parameters.MinimumPasswordLength)
        {
            // Direct array access instead of GetItems + First()
            string nextWord = words[random.Next(words.Count)];

            if (passwordBuilder.Length > 0)
            {
                // Capitalize first letter of subsequent words
                passwordBuilder.Append(char.ToUpper(nextWord[0]));
                passwordBuilder.Append(nextWord.AsSpan(1));
            }
            else
            {
                passwordBuilder.Append(nextWord);
            }
        }

        // Apply character replacements if requested
        if (parameters.DoSimpleReplacements)
        {
            ApplyReplacements(passwordBuilder);
        }

        return passwordBuilder.ToString();
    }

    internal static void ApplyReplacements(StringBuilder passwordBuilder)
    {
        // Single pass replacement using lookup table
        for (int i = 0; i < passwordBuilder.Length; i++)
        {
            if (SimpleReplacements.TryGetValue(passwordBuilder[i], out char replacement))
            {
                passwordBuilder[i] = replacement;
            }
        }
    }
}
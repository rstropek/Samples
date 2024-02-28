using System.Text;

namespace MeltingSnowperson;

class WordGuess
{
    private static readonly string[] AvailableWords = [
        "Snowflakes",
        "Frostbite",
        "Snowboarding",
        "Ice skating",
        "Thermometer",
        "Snowmobile",
        "Hibernation",
        "Blizzard",
        "Wintercoat",
        "Fireplace",
        "Snowstorm",
        "Ice fishing",
        "Scarves",
        "Frostwork",
        "Windchill",
        "Snowshoes",
        "Ice crystals",
        "Freezing rain",
        "Snowplough",
        "Antifreeze"
    ];

    public static WordGuess Create() => new(Random.Shared.GetItems(AvailableWords, 1)[0]);

    public WordGuess(string wordToGuess)
    {
        WordToGuess = wordToGuess;
        
        // Every letter in the word is replaced with an underscore. Blanks stay blanks.
        CurrentGuess = new string(WordToGuess.Select(c => !char.IsWhiteSpace(c) ? '_' : c).ToArray());
    }

    private string WordToGuess { get; }

    public string CurrentGuess { get; private set; }

    public int NumberOfWrongGuesses { get; private set; }

    public bool GuessLetter(char letter)
    {
        if (!char.IsLetter(letter))
        {
            return false;
        }

        var letterLower = char.ToLower(letter);

        var sb = new StringBuilder(CurrentGuess);
        bool found = false;
        for (int i = 0; i < WordToGuess.Length; i++)
        {
            if (char.ToLower(WordToGuess[i]) == letterLower)
            {
                sb[i] = WordToGuess[i];
                found = true;
            }
        }

        CurrentGuess = sb.ToString();
        if (!found)
        {
            NumberOfWrongGuesses++;
        }

        return found;
    }

    public bool Completed => CurrentGuess == WordToGuess;
}
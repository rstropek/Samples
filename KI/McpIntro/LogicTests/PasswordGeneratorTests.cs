using Logic;

namespace LogicTests;

public class PasswordGeneratorTests
{
    [Fact]
    public void BuildPassword_WithMinimumLength_ReturnsPasswordOfAtLeastMinimumLength()
    {
        // Arrange
        var customWords = new[] { "hello", "world", "test" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 10);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.True(password.Length >= 10);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(50)]
    public void BuildPassword_WithVariousLengths_ReturnsPasswordOfAtLeastSpecifiedLength(int minimumLength)
    {
        // Arrange
        var customWords = new[] { "short", "medium", "verylongword" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: minimumLength);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.True(password.Length >= minimumLength);
        Assert.NotEmpty(password);
    }

    [Fact]
    public void BuildPassword_WithSeed_ReturnsDeterministicPassword()
    {
        // Arrange
        var customWords = new[] { "apple", "banana", "cherry", "date" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 10, Seed: 42);

        // Act
        var password1 = PasswordGenerator.BuildPassword(parameters, customWords);
        var password2 = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.Equal(password1, password2);
    }

    [Fact]
    public void BuildPassword_WithDifferentSeeds_ReturnsDifferentPasswords()
    {
        // Arrange
        var customWords = new[] { "apple", "banana", "cherry", "date", "elderberry" };
        var parameters1 = new BuildPasswordParameters(MinimumPasswordLength: 10, Seed: 42);
        var parameters2 = new BuildPasswordParameters(MinimumPasswordLength: 10, Seed: 123);

        // Act
        var password1 = PasswordGenerator.BuildPassword(parameters1, customWords);
        var password2 = PasswordGenerator.BuildPassword(parameters2, customWords);

        // Assert
        Assert.NotEqual(password1, password2);
    }

    [Fact]
    public void BuildPassword_WithSimpleReplacements_ReplacesCharactersCorrectly()
    {
        // Arrange
        var customWords = new[] { "aestoit" }; // Contains all replaceable characters: a->@, e->3, s->$, o->0, i->1, t->7
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 6, DoSimpleReplacements: true, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // Should replace a->@, e->3, s->$, o->0, i->1, t->7
        Assert.Contains('@', password); // a -> @
        Assert.Contains('3', password); // e -> 3
        Assert.Contains('$', password); // s -> $
        Assert.Contains('0', password); // o -> 0
        Assert.Contains('1', password); // i -> 1
        Assert.Contains('7', password); // t -> 7
    }

    [Fact]
    public void BuildPassword_WithoutSimpleReplacements_DoesNotReplaceCharacters()
    {
        // Arrange
        var customWords = new[] { "aestoit" }; // Contains all replaceable characters
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 6, DoSimpleReplacements: false, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // Should contain original characters, not replacements
        Assert.Contains('a', password);
        Assert.Contains('e', password);
        Assert.Contains('s', password);
        Assert.Contains('o', password);
        Assert.Contains('i', password);
        Assert.Contains('t', password);
    }

    [Fact]
    public void BuildPassword_WithMultipleWords_CapitalizesFirstLetterOfSubsequentWords()
    {
        // Arrange
        var customWords = new[] { "red", "blue", "green" }; // Short words to force multiple words
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 15, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // Should start with lowercase and contain capital letters for subsequent words
        Assert.True(char.IsLower(password[0]), "First character should be lowercase");
        var capitalCount = password.Count(char.IsUpper);
        Assert.True(capitalCount > 0, "Should have capitalized letters for subsequent words");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void BuildPassword_WithVeryShortLength_StillGeneratesValidPassword(int length)
    {
        // Arrange
        var customWords = new[] { "a", "bb", "ccc" }; // Very short words
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: length);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.True(password.Length >= length);
        Assert.NotEmpty(password);
    }

    [Fact]
    public void BuildPassword_WithZeroLength_GeneratesEmptyString()
    {
        // Arrange
        var customWords = new[] { "test" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 0);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // With zero minimum length, the algorithm generates an empty string
        Assert.Equal("", password);
    }

    [Fact]
    public void BuildPassword_ConsistentBehaviorAcrossMultipleCalls_WithSameSeed()
    {
        // Arrange
        var customWords = new[] { "consistent", "behavior", "test", "seed" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 20, DoSimpleReplacements: true, Seed: 12345);

        // Act
        var passwords = Enumerable.Range(0, 5)
            .Select(_ => PasswordGenerator.BuildPassword(parameters, customWords))
            .ToList();

        // Assert
        Assert.True(passwords.All(p => p == passwords[0]), "All passwords should be identical with the same seed");
    }

    [Fact]
    public void BuildPassword_WithLargeMinimumLength_GeneratesCorrectLength()
    {
        // Arrange
        var customWords = new[] { "large", "password", "generation", "test" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 100, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.True(password.Length >= 100);
        Assert.True(password.Length < 150, "Password should not be excessively long");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void BuildPassword_ReplacementParameterRespected(bool doReplacements)
    {
        // Arrange
        var customWords = new[] { "testpassword" }; // Contains 't', 'e', 's' which are replaceable
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 12, DoSimpleReplacements: doReplacements, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        if (doReplacements)
        {
            // Should contain replacement characters
            var hasReplacements = password.Any(c => "@$0137".Contains(c));
            Assert.True(hasReplacements, "Should contain replacement characters when enabled");
        }
        else
        {
            // Should contain original characters, not replacements  
            Assert.Contains('t', password);
            Assert.Contains('e', password);
            Assert.Contains('s', password);
        }
    }

    [Fact]
    public void BuildPassword_WithSingleCharacterWords_GeneratesValidPassword()
    {
        // Arrange
        var singleCharWords = new[] { "a", "b", "c", "d", "e" };
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 10, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, singleCharWords);

        // Assert
        Assert.True(password.Length >= 10);
        Assert.True(password.All(c => "abcdeABCDE".Contains(c)), "Password should only contain expected characters");
    }

    [Fact]
    public void BuildPassword_WithEmptyWordList_ThrowsException()
    {
        // Arrange
        var emptyWords = Array.Empty<string>();
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 10);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => PasswordGenerator.BuildPassword(parameters, emptyWords));
    }

    [Fact]
    public void BuildPassword_WithExactLengthWord_GeneratesExactLength()
    {
        // Arrange
        var customWords = new[] { "exactly" }; // 7 characters
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 7, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        Assert.Equal("exactly", password);
    }

    [Fact]
    public void BuildPassword_WordCapitalizationPattern_IsCorrect()
    {
        // Arrange
        var customWords = new[] { "word" }; // Will be repeated multiple times
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 16, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // Expected pattern: "wordWordWordWord"
        Assert.StartsWith("word", password);
        Assert.Contains("Word", password); // Subsequent words should be capitalized
        var wordCount = password.Length / 4; // Each "word" or "Word" is 4 characters
        var expectedCapitalizations = wordCount - 1; // All except first should be capitalized
        var actualCapitalizations = password.Count(char.IsUpper);
        Assert.Equal(expectedCapitalizations, actualCapitalizations);
    }

    [Fact]
    public void BuildPassword_AllReplacementCharacters_AreReplaced()
    {
        // Arrange
        var customWords = new[] { "AaEeSsOoIiTt" }; // All replacement chars in both cases
        var parameters = new BuildPasswordParameters(MinimumPasswordLength: 12, DoSimpleReplacements: true, Seed: 42);

        // Act
        var password = PasswordGenerator.BuildPassword(parameters, customWords);

        // Assert
        // A->@, a->@, E->3, e->3, S->$, s->$, O->0, o->0, I->1, i->1, T->7, t->7
        Assert.Equal("@@33$$001177", password); // Corrected expected replacement pattern
    }
}

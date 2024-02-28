using MeltingSnowperson;

var person = new Snowperson();
var word = WordGuess.Create();

// Clear screen and hide cursor
Console.CursorVisible = false;

// Game loop
do
{
    Console.Clear();

    // Draw Snowperson at coordinates (0, 0)
    var snowperson = person.GetImage(word.NumberOfWrongGuesses);
    Console.SetCursorPosition(0, 0);
    Console.WriteLine(snowperson);

    // Draw word to guess at coordinates (60, 5)
    Console.SetCursorPosition(60, 5);
    Console.Write(word.CurrentGuess);

    // Draw number of wrong guesses at coordinates (60, 10)
    Console.SetCursorPosition(60, 10);
    Console.Write($"Number of wrong guesses: {word.NumberOfWrongGuesses}");

    // Draw input prompt at coordinates (60, 15)
    Console.SetCursorPosition(60, 15);
    Console.Write("Type a letter");

    // Wait for user input
    var key = Console.ReadKey()!;

    word.GuessLetter(key.KeyChar);
}
while (!word.Completed);

// Clear screen and make cursor visible
Console.Clear();
Console.CursorVisible = true;

// Write congratulations with the number of wrong guesses
Console.WriteLine($"Congratulations! You guessed the word \"{word.CurrentGuess}\" with {word.NumberOfWrongGuesses} wrong guesses.\n");

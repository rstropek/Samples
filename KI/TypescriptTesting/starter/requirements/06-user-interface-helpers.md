# User Interface Module

Write a class `MasterMindConsole` that offers functions for our game's user interface. You can find details about the UI concept [here](./02-ui-concept.md).

The class must have the following functions:

* `printWelcomeMessage` - prints the welcome message to the user
* `askForGuess` - asks the user for a guess and returns the result. Check the input using the [`GuessingLogic` class](./03-checker.md).
* `printGuessHistory` - prints the guess history (collection of guesses with results)
* `printGameOver` - prints the game over message (includes the number of guesses) to the user

## Method signature

```ts
export class MasterMindConsole {
    function printWelcomeMessage(): void { }
    async function askForGuess(): Promise<string> { }
    function printGuessHistory(guesses: { correct: number, appearing: number }[]): void { }
    function printGameOver(numberOfGuesses: number): void { }
}
```

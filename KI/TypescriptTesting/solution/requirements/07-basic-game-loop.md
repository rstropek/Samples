# Basic Game Loop

Write a class `Game` that implements a basic game loop:

* Generate a [_hidden code_](./04-generate-hidden-code.md). Do **not** reveal the code to the user.
* Print welcome message
* Run the game loop:
    * Prompt the user for a guess using the [`MasterMindConsole` class](./06-user-interface-helpers.md).
    * [Check the guess](./03-evaluator.md) against the hidden code.
    * Add the guess and the result to a guessing history.
    * Show the user the guessing history using the [`MasterMindConsole` class](./06-user-interface-helpers.md).
    * Check if the game is over (i.e. if the guess is the hidden code). If it is, print the game over message using the [`MasterMindConsole` class](./06-user-interface-helpers.md).
    * If not, start next loop iteration.

## Testability

The class uses the required, external logic via existing interfaces. This is done to decouple the class from any implementation details. This makes the class testable.

## Acceptance Criteria

* Prints welcome message when the game starts.
* Prompts the user for a guess after the welcome message.
* Checks the guess against the hidden code.
* Prints the guessing history with the guess and the result.
* Repeats the game loop until the game is over.
* Prints game over message with correct number of guesses when the game is over.

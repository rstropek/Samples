Write the jest tests for the spec in @03-checker.md in src/guessingLogic.test.ts. You can find a working sample test in @dummy.test.ts .

Now write the code for the given specs. You can run the tests with `npm test`.

Extend @guessingLogic.ts with the required functionality from @04-evaluator.md . Write the unit tests for the generated code in @guessingLogic.test.ts . You can run the tests with `npm test`.

Extend @guessingLogic.ts with the required functionality from @05-generate-hidden-code.md . Write the unit tests for the generated code in @guessingLogic.test.ts . You can run the tests with `npm test`.

Write the code for @06-user-interface-helpers.md . Consider the UI concept in @02-ui-concept.md . Do not write tests for that class yet.

Add an interface `IGuessChecker` to @guessingLogic.ts containing just `isValidGuess`. In @masterMindConsole.ts , use the interface to decouple guessing logic and console.

Extract a separate method in @MasterMindConsole that generates a string with the dots based on a result. Write a test for this function (not for the rest of the class).

Write an interface for the @MasterMindConsole class that can be used to decouple callers from the implementation

Implement the logic for the requirements in @07-basic-game-loop.md with corresponding tests. Use Jest mocks to mock dependencies.

Finally, bring everything together in _src/app.ts_. Create instances of @GuessingLogic , @MasterMindConsole , and @Game and run the game so we can play it.

Add a cheat feature. If the uses guesses "cheat", the program should display the hidden code.


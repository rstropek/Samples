# Guess Checker

In the class `GuessingLogic`, write a function `isValidGuess` that checks whether a guess is a valid guess.

A guess is valid if:

* The guess is a string of digits 1..6
* The guess has a length of 4

## Arguments

* `guess`: The guess to check

## Return value

* `true` if the guess is valid
* `false` otherwise

## Method signature

```ts
export class GuessingLogic {
    function isValidGuess(guess: string): boolean {
        // ..
    }
}
```

## Acceptance Criteria

| Guess  | Result
|--------|-------
| 1234   | true
| 1256   | true
| 1111   | true
| 0067   | false
| 12356  | false
| a123   | false

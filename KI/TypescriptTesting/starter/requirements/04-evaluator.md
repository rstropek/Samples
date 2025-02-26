# Evaluator

In the class `GuessingLogic`, write a function `evaluateGuess` that evaluates a guess and returns the feedback for the user.

First, the method must calculate the number of digits that are correctly positioned (_correct_). Next, the method iterates over digits that are not correctly positioned and checks whether they appear in the secret sequence on a different position (_appearing_).

If there are duplicate digits in the guess, they cannot all be counted as correct or appearing unless they correspond to the same number of duplicate digits in the hidden code. For example, if the hidden code is 1-1-2-2 and the user guesses 1-1-1-2, the feedback will contain three correct digits for the first two 1s and the 2, but nothing for the third 2. No indication is given of the fact that the code also includes a second 2.

## Arguments

* `guess`: The guess to evaluate, a sequence of digits in a string.
* `secret`: The secret sequence, a sequence of digits in a string.

## Return value

An object with two fields:

* `correct`: The number of digits that are correctly positioned.
* `appearing`: The number of digits that are in the secret sequence, but in the wrong position.

## Method signature

```ts
export class GuessingLogic {
    function evaluateGuess(guess: string, secret: string): { correct: number, appearing: number } {
        // ..
    }
}

## Exceptions

Throw an error if any of the following conditions are **not** met:

* `secret` and `guess` must be a string of digits 1..6
* `guess` must be the same length as `secret`.

## Acceptance Criteria

| Code | Guess | Result
|------|-------|------------
| 1122 | 1111  | 🔴🔴
| 1111 | 2221  | 🔴
| 1122 | 1112  | 🔴🔴🔴
| 1546 | 1234  | 🔴⚪      
| 1546 | 1456  | 🔴🔴⚪⚪ 
| 1546 | 1465  | 🔴⚪⚪⚪ 
| 1546 | 1546  | 🔴🔴🔴🔴 
| 1111 | 2222  |
| 1234 | 4321  | ⚪⚪⚪⚪

# Hands-on Labs

## Introduction

These hands-on labs can be used to practice design and testing of C# solutions.

It is recommended to work in teams of two people. One writes the necessary implementation and the other one write the unit tests.

If there is enough time, consider doing the hands-on lab in changing teams with different obstacles. Examples:

* Write all tests first, then add the logic.
* After at least two minutes, run all tests. If all are green, commit. If not, revoke all changes. Start next implementation cycle.
* The team members are not allowed to talk.
* Use a different programming language than C#.

## Tic-Tac-Toe

### Functional Requirements

Implement a reusable class library for a [3 x 3 Tic-Tac-Toe game](https://en.wikipedia.org/wiki/Tic-Tac-Toe). The library has to contain the following functional requirements:

* Store the *current state* of the game in-memory.
  * The current game state consists of the grid content (placement of *X*s and *O*s) and the player (*X* or *O*) who is to take the next turn.

* Start a *new game*.
  * The grid has to be initally empty and the player (*X* or *O*) who is to take the first turn has to be random.

* *Apply a new turn* to the grid.
  * In case of an invalid turn, an appropriate execption has to be thrown.
  * A turn is considered invalid in the following cases:
    * Invalid coordinates passed as arguments (e.g. place element at coordinates 7/9)
    * Grid element already occupied (e.g. tried to place *O* in a grid element that already contains *X*)
    * Wrong player making the turn (e.g. *X* making the turn, but *O* would be next)
    * There is already a winner

* Find out whether there is a *winner*.
  * If there is, return the player (*X* or *O*) who has won.
  
* *Serialize* the game state into a Base64 string.

* *Deserialize* the game state from a Base64 string.
  * If the Base64 string contains invalid data, an appropriate exception has to be thrown.

### Non-Functional Requirements

* The class library must be usable for UIs written with *WinForms*, *WPF*, *Xamarin Forms*, and *ASP.NET Core* (e.g. used in a web API for a JavaScript-based version of the game).

* The implementation has to be free of compile-time warnings.

* All of the functional requirements have to be verified by appropriate unit tests. Aim for a code coverage of > 90%.

* The library has to be built automatically using *Azure DevOps*. Run the unit tests during build and collect test and code coverage results.

## Password Generator

### Functional Requirements

Implement a reusable class library for a password generator. It will be used in a password keeper (like e.g. *KeePass*). The library has to contain the following functional requirements:

* Generate a random password base on the given options (see next requirements).
  * For this example, you can use C#'s pseudo-random number generator `Random`.
  * The function has to throw an exeption in case of invalid options. Options are considered invalid in the following cases:
    * No options specified
    * Invalid password length
    * Neither *mix uppercase and lowercase letters* nor *add special characters* are true. This is considered an unsecure combination of options.

* Possible options:
  * Password length (has to be between 5 and 25)
  * Add at least one number (0..9). If this is false, the generated password must not contain any numbers.
  * Mix uppercase and lowercase letters. If this is false, the generated password only contains lowercase letters. Otherwise, it has to contain uppercase and lowercase letters.
  * Add special characters (`!"§$%&/()={}#~`). If this is false, the generated password must not contain special characters. Otherwise, the generated password must contain at least one special character.
  * Apply Base64 encoding. If this is true, the password is Base64-encoded before it is returned.

* The library has to offer two sets of default password options. Callers can use these default options as a basis, optionally change them, and pass them to the password generator.
  * *Simple*
    * Password length: 10
    * Mix uppcase and lowercase: Yes
    * Special characters: No
    * Base64: No
  * *Strong*
    * Password length: 20
    * Mix uppcase and lowercase: Yes
    * Special characters: Yes
    * Base64: No

### Non-Functional Requirements

* The class library must be .NET Standard 2.1, so that it can be used in many different C# solutions.

* The implementation has to be free of compile-time warnings.

* All of the functional requirements have to be verified by appropriate unit tests. Aim for a code coverage of > 90%.

* The library has to be built automatically using *Azure DevOps*. Run the unit tests during build and collect test and code coverage results.

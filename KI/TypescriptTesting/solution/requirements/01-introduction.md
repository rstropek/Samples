# Console Mastermind

## Introduction

This document specifies the design and functionality of a console-based adaptation of the [Mastermind](https://en.wikipedia.org/wiki/Mastermind_(board_game)) game, utilizing numerical values (1..6) instead of traditional color-coded pegs.

The application is engineered to generate a secret sequence of numbers, which serves as the target combination for the user to decipher. The user is required to input a series of guesses, with each guess consisting of a sequence of numbers matching the length of the secret code. Following each guess, the system will evaluate the input and provide detailed feedback. This feedback will include indicators for:
 
- Numbers that are present in the secret sequence and correctly positioned.
- Numbers that are present in the secret sequence but incorrectly positioned.

The feedback mechanism is designed to assist the user in refining subsequent guesses, progressively narrowing down the possibilities until the correct sequence is identified. The primary objective is to achieve the correct numerical combination in the minimum number of attempts.

This specification outlines the core functionality of the application, establishing the foundation for further development and testing of the Mastermind game in a console environment.

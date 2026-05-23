# UI Concept

The following text shows an example of the expected user interface when running the application.

1. The user sees the history of their guesses and the corresponding feedback.
2. The user is prompted to enter their next guess.
3. The application evaluates the guess and provides feedback.
4. The user can make additional guesses until the guess is correct.

Note that red dots represent digits that are on the correct position (_correct_), and white dots represent digits that are in the secret code but on the wrong position (_appearing_).

```txt
| Guess | Result
|-------|------------

Enter your guess: 1234

| Guess | Result     
|-------|------------
| 1234  | 🔴⚪      

Enter your guess: 1456

| Guess | Result     
|-------|------------
| 1234  | 🔴⚪      
| 1456  | 🔴🔴⚪⚪ 

Enter your guess: 1465

| Guess | Result     
|-------|------------
| 1234  | 🔴⚪      
| 1456  | 🔴🔴⚪⚪ 
| 1465  | 🔴⚪⚪⚪ 

Enter your guess: 1546

| Guess | Result     
|-------|------------
| 1234  | 🔴⚪      
| 1456  | 🔴🔴⚪⚪ 
| 1465  | 🔴⚪⚪⚪ 
| 1546  | 🔴🔴🔴🔴 

Correct!
```


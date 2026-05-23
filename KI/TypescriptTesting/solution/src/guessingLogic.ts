export interface IGuessChecker {
    isValidGuess(guess: string): boolean;
}

export interface IGuessingLogic extends IGuessChecker {
    generateHiddenCode(): string;
    evaluateGuess(guess: string, secret: string): { correct: number, appearing: number };
}

export class GuessingLogic implements IGuessingLogic {
    isValidGuess(guess: string): boolean {
        // Use regex to check if the string is exactly 4 digits between 1-6
        return /^[1-6]{4}$/.test(guess);
    }

    evaluateGuess(guess: string, secret: string): { correct: number, appearing: number } {
        // Validate inputs
        if (!this.isValidGuess(guess) || !this.isValidGuess(secret)) {
            throw new Error('Both guess and secret must be valid 4-digit codes (digits 1-6)');
        }

        if (guess.length !== secret.length) {
            throw new Error('Guess must be the same length as secret');
        }

        let correct = 0;
        let appearing = 0;

        // Count correct positions first
        const unusedSecret = secret.split('');
        const unusedGuess = guess.split('');
        
        // First pass: Find correct positions
        for (let i = unusedGuess.length - 1; i >= 0; i--) {
            if (unusedGuess[i] === unusedSecret[i]) {
                correct++;
                unusedGuess.splice(i, 1);
                unusedSecret.splice(i, 1);
            }
        }

        // Second pass: Find appearing numbers
        for (let i = 0; i < unusedGuess.length; i++) {
            const pos = unusedSecret.indexOf(unusedGuess[i]);
            if (pos !== -1) {
                appearing++;
                unusedSecret.splice(pos, 1); // Remove the matched digit to handle duplicates correctly
            }
        }

        return { correct, appearing };
    }

    generateHiddenCode(): string {
        const digits = [];
        for (let i = 0; i < 4; i++) {
            // Generate random number between 1 and 6 (inclusive)
            digits.push(Math.floor(Math.random() * 6) + 1);
        }
        return digits.join('');
    }
}

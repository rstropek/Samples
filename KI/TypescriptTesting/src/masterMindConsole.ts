import { IGuessChecker, GuessingLogic } from './guessingLogic.js';
import * as readline from 'readline';

export interface IMasterMindConsole {
    printWelcomeMessage(): void;
    askForGuess(): Promise<string>;
    generateResultDots(result: { correct: number, appearing: number }): string;
    printGuessHistory(guesses: { guess: string, result: { correct: number, appearing: number } }[]): void;
    printGameOver(numberOfGuesses: number): void;
    close(): void;
}

export class MasterMindConsole implements IMasterMindConsole {
    private readonly guessingLogic: IGuessChecker;
    private readonly rl: readline.Interface;

    constructor() {
        this.guessingLogic = new GuessingLogic();
        this.rl = readline.createInterface({
            input: process.stdin,
            output: process.stdout
        });
    }

    printWelcomeMessage(): void {
        console.log('| Guess | Result');
        console.log('|-------|------------');
        console.log();
    }

    async askForGuess(): Promise<string> {
        return new Promise((resolve) => {
            const askForInput = () => {
                this.rl.question('Enter your guess: ', (guess) => {
                    if (this.guessingLogic.isValidGuess(guess)) {
                        resolve(guess);
                    } else {
                        console.log('Invalid guess. Please enter a 4-digit number using digits 1-6.');
                        askForInput();
                    }
                });
            };
            askForInput();
            console.log();
        });
    }

    generateResultDots(result: { correct: number, appearing: number }): string {
        return '🔴'.repeat(result.correct) + '⚪'.repeat(result.appearing);
    }

    printGuessHistory(guesses: { guess: string, result: { correct: number, appearing: number } }[]): void {
        console.log('| Guess | Result');
        console.log('|-------|------------');
        
        for (const entry of guesses) {
            const dots = this.generateResultDots(entry.result);
            console.log(`| ${entry.guess}  | ${dots}`);
        }
        console.log();
    }

    printGameOver(numberOfGuesses: number): void {
        console.log(`Correct! You solved it in ${numberOfGuesses} ${numberOfGuesses === 1 ? 'guess' : 'guesses'}!`);
        this.rl.close();
    }

    // Helper method for cleanup in tests
    close(): void {
        this.rl.close();
    }
}

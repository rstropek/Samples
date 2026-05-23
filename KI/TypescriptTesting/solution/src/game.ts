import { IGuessingLogic } from './guessingLogic.js';
import { IMasterMindConsole } from './masterMindConsole.js';

export class Game {
    private readonly guessingLogic: IGuessingLogic;
    private readonly console: IMasterMindConsole;
    private guessHistory: { guess: string, result: { correct: number, appearing: number } }[] = [];
    private hiddenCode: string = '';

    constructor(guessingLogic: IGuessingLogic, console: IMasterMindConsole) {
        this.guessingLogic = guessingLogic;
        this.console = console;
    }

    public async play(): Promise<void> {
        this.hiddenCode = this.guessingLogic.generateHiddenCode();
        this.console.printWelcomeMessage();

        while (true) {
            const guess = await this.console.askForGuess();
            
            if (guess.toLowerCase() === 'cheat') {
                this.console.printCheatCode(this.hiddenCode);
                continue;
            }

            const result = this.guessingLogic.evaluateGuess(guess, this.hiddenCode);
            
            this.guessHistory.push({ guess, result });
            this.console.printGuessHistory(this.guessHistory);

            if (result.correct === 4) {
                this.console.printGameOver(this.guessHistory.length);
                this.console.close();
                break;
            }
        }
    }
}

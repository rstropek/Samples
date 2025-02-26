import { Game } from './game.js';
import { IGuessingLogic } from './guessingLogic.js';
import { IMasterMindConsole } from './masterMindConsole.js';

describe('Game', () => {
    let mockGuessingLogic: jest.Mocked<IGuessingLogic>;
    let mockConsole: jest.Mocked<IMasterMindConsole>;
    let game: Game;

    beforeEach(() => {
        jest.useFakeTimers();
        
        mockGuessingLogic = {
            generateHiddenCode: jest.fn(),
            evaluateGuess: jest.fn(),
            isValidGuess: jest.fn(),
        };

        mockConsole = {
            printWelcomeMessage: jest.fn(),
            askForGuess: jest.fn(),
            generateResultDots: jest.fn(),
            printGuessHistory: jest.fn(),
            printGameOver: jest.fn(),
            printCheatCode: jest.fn(),
            close: jest.fn(),
        };

        game = new Game(mockGuessingLogic, mockConsole);
    });

    afterEach(() => {
        jest.useRealTimers();
    });

    it('should print welcome message and generate hidden code when game starts', async () => {
        mockGuessingLogic.generateHiddenCode.mockReturnValue('1234');
        mockConsole.askForGuess.mockResolvedValue('1234');
        mockGuessingLogic.evaluateGuess.mockReturnValue({ correct: 4, appearing: 0 });

        await game.play();

        expect(mockConsole.printWelcomeMessage).toHaveBeenCalled();
        expect(mockGuessingLogic.generateHiddenCode).toHaveBeenCalled();
    });

    it('should continue game loop until correct guess is made', async () => {
        mockGuessingLogic.generateHiddenCode.mockReturnValue('1234');
        mockConsole.askForGuess
            .mockResolvedValueOnce('5612')
            .mockResolvedValueOnce('1234');
        
        mockGuessingLogic.evaluateGuess
            .mockReturnValueOnce({ correct: 1, appearing: 1 })
            .mockReturnValueOnce({ correct: 4, appearing: 0 });

        await game.play();

        expect(mockConsole.askForGuess).toHaveBeenCalledTimes(2);
        expect(mockConsole.printGuessHistory).toHaveBeenCalledTimes(2);
        expect(mockConsole.printGameOver).toHaveBeenCalledWith(2);
        expect(mockConsole.close).toHaveBeenCalled();
    });

    it('should update and print guess history after each guess', async () => {
        mockGuessingLogic.generateHiddenCode.mockReturnValue('1234');
        mockConsole.askForGuess
            .mockResolvedValueOnce('5612')
            .mockResolvedValue('1234');
        mockGuessingLogic.evaluateGuess
            .mockReturnValueOnce({ correct: 1, appearing: 1 })
            .mockReturnValue({ correct: 4, appearing: 0 });

        const expectedHistory = [{
            guess: '5612',
            result: { correct: 1, appearing: 1 }
        }];

        const playPromise = game.play();
        
        // Fast-forward until all timers have been executed
        jest.runAllTimers();
        
        // Resolve all pending promises
        await Promise.resolve();
        
        expect(mockConsole.printGuessHistory).toHaveBeenCalledWith(expectedHistory);
        
        const expectedHistory2 = [{
            guess: '5612',
            result: { correct: 1, appearing: 1 }
        },
        {
            guess: '1234',
            result: { correct: 4, appearing: 0 }
        }];

        jest.runAllTimers();
        await Promise.resolve();
        expect(mockConsole.printGuessHistory).toHaveBeenCalledWith(expectedHistory2);

        // Complete the game
        await playPromise;
    });

    it('should handle cheat command', async () => {
        mockGuessingLogic.generateHiddenCode.mockReturnValue('1234');
        mockConsole.askForGuess
            .mockResolvedValueOnce('cheat')
            .mockResolvedValueOnce('1234');
        mockGuessingLogic.evaluateGuess.mockReturnValue({ correct: 4, appearing: 0 });

        await game.play();

        expect(mockConsole.printCheatCode).toHaveBeenCalledWith('1234');
        expect(mockConsole.printGameOver).toHaveBeenCalledWith(1);
    });
});

import { describe, expect, test } from '@jest/globals';
import { GuessingLogic } from './guessingLogic.js';

describe('GuessingLogic - isValidGuess', () => {
    const logic = new GuessingLogic();

    test.each([
        ['1234', true, 'valid 4-digit code'],
        ['1256', true, 'valid 4-digit code with different numbers'],
        ['1111', true, 'valid 4-digit code with repeated numbers'],
        ['0067', false, 'invalid code with zero'],
        ['12356', false, 'code too long'],
        ['a123', false, 'code with letters'],
        ['', false, 'empty string'],
        ['123', false, 'code too short'],
        ['1#34', false, 'code with special characters'],
        ['1 34', false, 'code with spaces'],
        ['1789', false, 'code with digits greater than 6']
    ])('isValidGuess("%s") should return %s (%s)', (guess, expected, description) => {
        expect(logic.isValidGuess(guess)).toBe(expected);
    });
});

describe('GuessingLogic - evaluateGuess', () => {
    const logic = new GuessingLogic();

    // Test acceptance criteria cases
    test.each([
        ['1122', '1111', { correct: 2, appearing: 0 }, 'two correct positions'],
        ['1111', '2221', { correct: 1, appearing: 0 }, 'one correct position'],
        ['1122', '1112', { correct: 3, appearing: 0 }, 'three correct positions'],
        ['1546', '1234', { correct: 1, appearing: 1 }, 'one correct position'],
        ['1546', '1456', { correct: 2, appearing: 2 }, 'two correct, two appearing'],
        ['1546', '1465', { correct: 1, appearing: 3 }, 'one correct, three appearing'],
        ['1546', '1546', { correct: 4, appearing: 0 }, 'all correct']
    ])('evaluateGuess("%s", "%s") should return %j (%s)', 
        (secret, guess, expected, description) => {
            expect(logic.evaluateGuess(guess, secret)).toEqual(expected);
    });

    // Test error cases
    test('should throw error for invalid guess', () => {
        expect(() => logic.evaluateGuess('1234', '7890')).toThrow();
    });

    test('should throw error for invalid secret', () => {
        expect(() => logic.evaluateGuess('7890', '1234')).toThrow();
    });

    test('should throw error for different lengths', () => {
        expect(() => logic.evaluateGuess('123', '1234')).toThrow();
    });
});

describe('GuessingLogic - generateHiddenCode', () => {
    const logic = new GuessingLogic();

    test('generates a valid 4-digit code', () => {
        const code = logic.generateHiddenCode();
        expect(logic.isValidGuess(code)).toBe(true);
    });

    test('generates different codes', () => {
        const codes = new Set();
        for (let i = 0; i < 10; i++) {
            codes.add(logic.generateHiddenCode());
        }
        // With 10 attempts, we should get at least 2 different codes
        // (probability of getting the same code 10 times is extremely low)
        expect(codes.size).toBeGreaterThan(1);
    });
});

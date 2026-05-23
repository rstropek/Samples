import { describe, expect, test } from '@jest/globals';
import { MasterMindConsole } from './masterMindConsole.js';

describe('MasterMindConsole - generateResultDots', () => {
    const console = new MasterMindConsole();

    test.each([
        [{ correct: 0, appearing: 0 }, '', 'no correct or appearing digits'],
        [{ correct: 1, appearing: 0 }, '🔴', 'one correct digit'],
        [{ correct: 0, appearing: 1 }, '⚪', 'one appearing digit'],
        [{ correct: 2, appearing: 2 }, '🔴🔴⚪⚪', 'multiple correct and appearing digits'],
        [{ correct: 4, appearing: 0 }, '🔴🔴🔴🔴', 'all correct digits'],
        [{ correct: 0, appearing: 4 }, '⚪⚪⚪⚪', 'all appearing digits']
    ])('generateResultDots(%o) should return "%s" (%s)', (result, expected, description) => {
        expect(console.generateResultDots(result)).toBe(expected);
    });

    afterAll(() => {
        console.close();
    });
});

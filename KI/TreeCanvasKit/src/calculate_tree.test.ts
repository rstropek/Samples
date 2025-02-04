import { calculateBranchPoints, calculateFractalTreePoints } from './calculate_tree';

describe('calculateBranchPoints', () => {
    test('should calculate endpoint correctly for 0 degrees (right)', () => {
        const result = calculateBranchPoints(0, 0, 0, 10);
        expect(result.x1).toBe(0);
        expect(result.y1).toBe(0);
        expect(result.x2).toBeCloseTo(10); // cos(0) * 10 = 10
        expect(result.y2).toBeCloseTo(0);  // sin(0) * 10 = 0
    });

    test('should calculate endpoint correctly for 90 degrees (up)', () => {
        const result = calculateBranchPoints(0, 0, 90, 10);
        expect(result.x1).toBe(0);
        expect(result.y1).toBe(0);
        expect(result.x2).toBeCloseTo(0);  // cos(90) * 10 = 0
        expect(result.y2).toBeCloseTo(10); // sin(90) * 10 = 10
    });

    test('should calculate endpoint correctly for -90 degrees (down)', () => {
        const result = calculateBranchPoints(0, 0, -90, 10);
        expect(result.x1).toBe(0);
        expect(result.y1).toBe(0);
        expect(result.x2).toBeCloseTo(0);   // cos(-90) * 10 = 0
        expect(result.y2).toBeCloseTo(-10); // sin(-90) * 10 = -10
    });

    test('should calculate endpoint correctly for 45 degrees (diagonal)', () => {
        const result = calculateBranchPoints(0, 0, 45, 10);
        const expectedX = 7.071; // cos(45) * 10 ≈ 7.071
        const expectedY = 7.071; // sin(45) * 10 ≈ 7.071
        expect(result.x1).toBe(0);
        expect(result.y1).toBe(0);
        expect(result.x2).toBeCloseTo(expectedX, 3);
        expect(result.y2).toBeCloseTo(expectedY, 3);
    });

    test('should work with non-zero starting points', () => {
        const result = calculateBranchPoints(5, 5, 0, 10);
        expect(result.x1).toBe(5);
        expect(result.y1).toBe(5);
        expect(result.x2).toBeCloseTo(15); // 5 + cos(0) * 10 = 15
        expect(result.y2).toBeCloseTo(5);  // 5 + sin(0) * 10 = 5
    });
});

describe('calculateFractalTreePoints', () => {
    test('should return empty array when level equals maxLevel', () => {
        const params = {
            x1: 0,
            y1: 0,
            angle: 0,
            length: 10,
            level: 11,
            maxLevel: 11
        };
        const result = calculateFractalTreePoints(params);
        expect(result).toHaveLength(0);
    });

    test('should return one branch for level 0', () => {
        const params = {
            x1: 0,
            y1: 0,
            angle: 90,
            length: 10,
            level: 0,
            maxLevel: 1
        };
        const result = calculateFractalTreePoints(params);
        expect(result).toHaveLength(1); // Only the main branch
    });

    test('should return three branches for level 1 (main branch + two children)', () => {
        const params = {
            x1: 0,
            y1: 0,
            angle: 90,
            length: 10,
            level: 0,
            maxLevel: 2
        };
        const result = calculateFractalTreePoints(params);
        expect(result).toHaveLength(3); // Main branch + left branch + right branch
    });

    test('should return seven branches for level 2', () => {
        const params = {
            x1: 0,
            y1: 0,
            angle: 90,
            length: 10,
            level: 0,
            maxLevel: 3
        };
        const result = calculateFractalTreePoints(params);
        expect(result).toHaveLength(7); // 1 main + 2 level1 + 4 level2 branches
    });
}); 
import { degreeToRadian } from './math_helper';


export type LinePoint = {
    x1: number;
    y1: number;
    x2: number;
    y2: number;
}

export function calculateBranchPoints(x1: number, y1: number, angle: number, length: number): LinePoint {
    const x2 = x1 + Math.cos(degreeToRadian(angle)) * length;
    const y2 = y1 + Math.sin(degreeToRadian(angle)) * length;
    return { x1, y1, x2, y2 };
}

export interface TreeParams {
    x1: number;
    y1: number;
    angle: number;
    length: number;
    level: number;
    maxLevel?: number;
}

export function calculateFractalTreePoints(params: TreeParams): LinePoint[] {
    const { x1, y1, angle, length, level, maxLevel = 11 } = params;
    if (level >= maxLevel) { return []; }

    const currentBranch = calculateBranchPoints(x1, y1, angle, length);
    const points: LinePoint[] = [currentBranch];

    const leftBranches = calculateFractalTreePoints({
        x1: currentBranch.x2,
        y1: currentBranch.y2,
        angle: angle - 20,
        length: length * 0.8,
        level: level + 1,
        maxLevel
    });
    const rightBranches = calculateFractalTreePoints({
        x1: currentBranch.x2,
        y1: currentBranch.y2,
        angle: angle + 20,
        length: length * 0.8,
        level: level + 1,
        maxLevel
    });

    return [...points, ...leftBranches, ...rightBranches];
}

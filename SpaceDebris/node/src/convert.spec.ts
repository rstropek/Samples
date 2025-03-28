import {describe, expect, test} from '@jest/globals';
import { dmsToDd, llhToEcef, ecefDistance } from './convert';

describe('dmsToDd', () => {
    test('northern hemisphere', () => {
        expect(dmsToDd("11°37'44N")).toBe(11.628889);
    });

    test('western hemisphere', () => {
        expect(dmsToDd("145°50'21W")).toBe(-145.839167);
    });

    test('southern hemisphere', () => {
        expect(dmsToDd("12°30'00S")).toBe(-12.5);
    });

    test('west zero minutes seconds', () => {
        expect(dmsToDd("74°00'00W")).toBe(-74.0);
    });

    test('eastern hemisphere', () => {
        expect(dmsToDd("120°45'30E")).toBe(120.758333);
    });

    test('near equator', () => {
        expect(dmsToDd("00°00'01N")).toBe(0.000278);
    });

    test('invalid format', () => {
        expect(() => dmsToDd("invalid")).toThrow('Invalid DMS format: invalid');
    });
});

describe('llhToEcef', () => {
    test('direct coordinates', () => {
        const [x, y, z] = llhToEcef(11.6288890, -145.8391670, 1795.59);
        expect(x).toBe(-6625344.32);
        expect(y).toBe(-4495961.62);
        expect(z).toBe(1639159.98);
    });

    test('with dms conversion', () => {
        const lat = dmsToDd("90°3'28N");
        const lon = dmsToDd("143°42'18W");
        const [x, y, z] = llhToEcef(lat, lon, 1245.93);
        expect(x).toBe(6214.0);
        expect(y).toBe(4563.8);
        expect(z).toBe(7602678.43);
    });
});

describe('ecefDistance', () => {
    test('distance between identical points is zero', () => {
        const point: [number, number, number] = [100.0, 200.0, 300.0];
        expect(ecefDistance(point, point)).toBe(0.0);
    });

    test('distance with simple coordinate values', () => {
        const point1: [number, number, number] = [0.0, 0.0, 0.0];
        const point2: [number, number, number] = [3.0, 4.0, 0.0];
        expect(ecefDistance(point1, point2)).toBe(5.0);
    });

    test('distance between actual ECEF coordinates', () => {
        const point1: [number, number, number] = [-6625344.32, -4495961.62, 1639159.98];
        const point2: [number, number, number] = [6214.0, 4563.8, 7602678.43];
        const expected = 9989787.14;
        expect(Number(ecefDistance(point1, point2).toFixed(2))).toBe(expected);
    });
});

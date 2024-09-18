import { describe, it, expect } from '@jest/globals';
import { toTime, roundToNearestFive } from './time';

describe('toTime function', () => {
  it('should convert midnight to 12:00 format', () => {
    const date = new Date('2023-01-01T00:00:00');
    expect(toTime(date)).toEqual({ h: 12, m: 0 });
  });

  it('should convert noon to 12:00 format', () => {
    const date = new Date('2023-01-01T12:00:00');
    expect(toTime(date)).toEqual({ h: 12, m: 0 });
  });

  it('should correctly convert a time in the PM to 12-hour format', () => {
    const date = new Date('2023-01-01T15:30:00');
    expect(toTime(date)).toEqual({ h: 3, m: 30 });
  });

  it('should correctly convert a time in the AM to 12-hour format', () => {
    const date = new Date('2023-01-01T03:20:00');
    expect(toTime(date)).toEqual({ h: 3, m: 20 });
  });
});

describe('roundToNearestFive function', () => {
  it('should round minutes down to the nearest five', () => {
    expect(roundToNearestFive({ h: 14, m: 32 })).toEqual({ h: 14, m: 30 });
  });

  it('should round minutes up to the nearest five', () => {
    expect(roundToNearestFive({ h: 9, m: 33 })).toEqual({ h: 9, m: 35 });
  });

  it('should round minutes up to 60 and increase hour', () => {
    expect(roundToNearestFive({ h: 10, m: 58 })).toEqual({ h: 11, m: 0 });
  });

  it('should handle rounding up at 23 hours', () => {
    expect(roundToNearestFive({ h: 23, m: 58 })).toEqual({ h: 0, m: 0 });
  });

  it('should not change hours if minutes are exactly on a five', () => {
    expect(roundToNearestFive({ h: 5, m: 55 })).toEqual({ h: 5, m: 55 });
  });
});


import { describe, it, expect } from '@jest/globals';
import { setActiveSegments } from './wordIds';
import { time } from './time';

describe('setActiveSegments', () => {
  it('should activate "it", "is", and "oclock" at exactly an hour', () => {
    const currentTime: time = { h: 1, m: 0 };
    const result = setActiveSegments(currentTime);
    expect(result).toEqual({
      it: true,
      is: true,
      one: true,
      oclock: true,
    });
  });

  it('should activate "past" and relevant minute and hour segments before half past', () => {
    const currentTime: time = { h: 2, m: 20 };
    const result = setActiveSegments(currentTime);
    expect(result).toEqual({
      it: true,
      is: true,
      twenty: true,
      minutes: true,
      past: true,
      two: true,
    });
  });

  it('should activate "to", "minutes", and adjust hour segment after half past', () => {
    const currentTime: time = { h: 3, m: 40 };
    const result = setActiveSegments(currentTime);
    expect(result).toEqual({
      it: true,
      is: true,
      twenty: true,
      minutes: true,
      to: true,
      four: true, // Note: hour is adjusted to the next hour
    });
  });

  it('should handle "quarter" past and to the hour', () => {
    const currentTimeQuarterPast = { h: 4, m: 15 };
    const currentTimeQuarterTo = { h: 4, m: 45 };
    const resultQuarterPast = setActiveSegments(currentTimeQuarterPast);
    const resultQuarterTo = setActiveSegments(currentTimeQuarterTo);
    expect(resultQuarterPast).toEqual({
      it: true,
      is: true,
      quarter: true,
      past: true,
      four: true,
    });
    expect(resultQuarterTo).toEqual({
      it: true,
      is: true,
      quarter: true,
      to: true,
      five_hour: true, // Note: hour is adjusted to the next hour
    });
  });

  it('should activate "half" at half past the hour', () => {
    const currentTime: time = { h: 5, m: 30 };
    const result = setActiveSegments(currentTime);
    expect(result).toEqual({
      it: true,
      is: true,
      half: true,
      past: true,
      five_hour: true,
    });
  });

  it('should correctly handle transition from "to" next hour at 55 minutes', () => {
    const currentTime: time = { h: 10, m: 55 };
    const result = setActiveSegments(currentTime);
    expect(result).toEqual({
      it: true,
      is: true,
      five: true,
      minutes: true,
      to: true,
      eleven: true, // Note: hour is adjusted to the next hour
    });
  });
});

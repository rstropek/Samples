import { describe, it, expect } from '@jest/globals';
import { Segments, getSegmentBit, isValidSegment } from './SegmentBits';

describe('isValidSegment', () => {
  it('should return true for valid segments', () => {
    expect(isValidSegment(Segments.A)).toBe(true);
    expect(isValidSegment(Segments.B)).toBe(true);
    expect(isValidSegment(Segments.C)).toBe(true);
    expect(isValidSegment(Segments.D)).toBe(true);
    expect(isValidSegment(Segments.E)).toBe(true);
    expect(isValidSegment(Segments.F)).toBe(true);
    expect(isValidSegment(Segments.G)).toBe(true);
  });

  it('should return false for invalid segments', () => {
    expect(isValidSegment(<any>0b00000000)).toBe(false);
    expect(isValidSegment(<any>0b11111111)).toBe(false);
    expect(isValidSegment(<any>0b10101010)).toBe(false);
  });
});

describe('getSegmentBit', () => {
  it('should return true if the segment bit is set', () => {
    expect(getSegmentBit(0b0000001, Segments.A)).toBe(true);
    expect(getSegmentBit(0b0000010, Segments.B)).toBe(true);
    expect(getSegmentBit(0b0000100, Segments.C)).toBe(true);
    expect(getSegmentBit(0b0001000, Segments.D)).toBe(true);
    expect(getSegmentBit(0b0010000, Segments.E)).toBe(true);
    expect(getSegmentBit(0b0100000, Segments.F)).toBe(true);
    expect(getSegmentBit(0b1000000, Segments.G)).toBe(true);
  });

  it('should return false if the segment bit is not set', () => {
    expect(getSegmentBit(0b0000000, Segments.A)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.B)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.C)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.D)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.E)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.F)).toBe(false);
    expect(getSegmentBit(0b0000000, Segments.G)).toBe(false);
  });

  it('should throw an error for invalid segments', () => {
    expect(() => getSegmentBit(0b0000000, <any>0b11111111)).toThrow("Invalid segment");
  });
});

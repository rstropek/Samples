export enum Segments {
  A = 0b0000001,
  B = 0b0000010,
  C = 0b0000100,
  D = 0b0001000,
  E = 0b0010000,
  F = 0b0100000,
  G = 0b1000000,
}

export function isValidSegment(segment: Segments) {
  return Object.values(Segments).includes(segment);
}

export function getSegmentBit(digit: number, segment: Segments) {
  if (!isValidSegment(segment)) {
    throw new Error("Invalid segment");
  }

  return (digit & segment) === segment;
}

export function setSegmentBit(digit: number, segment: Segments) {
  if (!isValidSegment(segment)) {
    throw new Error("Invalid segment");
  }

  return digit | segment;
}

export function clearSegmentBit(digit: number, segment: Segments) {
  if (!isValidSegment(segment)) {
    throw new Error("Invalid segment");
  }

  return digit & ~segment;
}

export function getSegmentBitsForDigit(digit: number) {
  switch (digit) {
    case 0:
      return Segments.A | Segments.B | Segments.C | Segments.D | Segments.E | Segments.F;
    case 1:
      return Segments.B | Segments.C;
    case 2:
      return Segments.A | Segments.B | Segments.G | Segments.E | Segments.D;
    case 3:
      return Segments.A | Segments.B | Segments.C | Segments.D | Segments.G;
    case 4:
      return Segments.F | Segments.B | Segments.G | Segments.C;
    case 5:
      return Segments.A | Segments.F | Segments.G | Segments.C | Segments.D;
    case 6:
      return Segments.A | Segments.F | Segments.G | Segments.C | Segments.D | Segments.E;
    case 7:
      return Segments.A | Segments.B | Segments.C;
    case 8:
      return Segments.A | Segments.B | Segments.C | Segments.D | Segments.E | Segments.F | Segments.G;
    case 9:
      return Segments.A | Segments.B | Segments.C | Segments.D | Segments.F | Segments.G;
    default:
      throw new Error("Invalid digit");
  }
}

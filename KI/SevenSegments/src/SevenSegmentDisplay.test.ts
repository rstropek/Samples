import { jest, describe, it, expect, beforeEach } from '@jest/globals';
import { drawLineOfDigit } from './SevenSegmentDisplay';
import { Terminal } from './Terminal';
import { Segments } from './SegmentBits';

jest.mock('./Terminal', () => {
  return {
    Terminal: jest.fn().mockImplementation(() => {
      return {
        write: jest.fn(),
        clearScreen: jest.fn(),
        writeNewline: jest.fn(),
      };
    }),
  };
});

describe('drawLineOfDigit', () => {
  let terminal: Terminal;

  beforeEach(() => {
    terminal = new Terminal();
  });

  it('should draw line of digit correctly', () => {
    drawLineOfDigit(1, 0, terminal);
    expect(terminal.write).toHaveBeenCalled();
  });

  it('should draw line 0 of digit 8 correctly', () => {
    drawLineOfDigit(8, 0, terminal);

    expect(terminal.write).toHaveBeenCalledTimes(10);
    expect((<jest.Mock>terminal.write).mock.calls).toEqual([[' '], ['*'], ['*'], ['*'], ['*'], ['*'], ['*'], ['*'], ['*'], [' ']]);
  });
});

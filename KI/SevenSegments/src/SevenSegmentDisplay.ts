import { Segments, getSegmentBitsForDigit } from './SegmentBits';
import { Terminal } from './Terminal';

export function drawLineOfDigit(digit: number, line: number, terminal: Terminal) {
  const bits = getSegmentBitsForDigit(digit);

  switch (line) {
    case 0:
      terminal.write(' ');
      for (let i = 0; i < 8; i++) {
        if (bits & Segments.A) {
          terminal.write('*');
        } else {
          terminal.write(' ');
        }
      }
      terminal.write(' ');
      break;
    case 1:
    case 2:
    case 3:
      if (bits & Segments.F) {
        terminal.write('*');
      } else {
        terminal.write(' ');
      }
      for (let i = 0; i < 8; i++) {
        terminal.write(' ');
      }
      if (bits & Segments.B) {
        terminal.write('*');
      } else {
        terminal.write(' ');
      }
      break;
    case 4:
      terminal.write(' ');
      for (let i = 0; i < 8; i++) {
        if (bits & Segments.G) {
          terminal.write('*');
        } else {
          terminal.write(' ');
        }
      }
      terminal.write(' ');
      break;
    case 5:
    case 6:
    case 7:
      if (bits & Segments.E) {
        terminal.write('*');
      } else {
        terminal.write(' ');
      }
      for (let i = 0; i < 8; i++) {
        terminal.write(' ');
      }
      if (bits & Segments.C) {
        terminal.write('*');
      } else {
        terminal.write(' ');
      }
      break;
    case 8:
      terminal.write(' ');
      for (let i = 0; i < 8; i++) {
        if (bits & Segments.D) {
          terminal.write('*');
        } else {
          terminal.write(' ');
        }
      }
      terminal.write(' ');
      break;
  }
}

export function drawNumber(value: number, terminal: Terminal) {
  // How many digits are in the number?
  let numDigits = 1;
  let temp = value;
  while (temp >= 10) {
    temp = Math.floor(temp / 10);
    numDigits++;
  }

  for (let l = 0; l < 9; l++) {
    for (let i = 0; i < numDigits; i++) {
      const digit = Math.floor(value / Math.pow(10, numDigits - i - 1)) % 10;
      drawLineOfDigit(digit, l, terminal);
      terminal.write(' ');
    }
    terminal.writeNewline();
  }
}

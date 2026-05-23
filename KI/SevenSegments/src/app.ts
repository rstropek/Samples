import { drawLineOfDigit, drawNumber } from "./SevenSegmentDisplay";
import { Terminal } from './Terminal';

const t = new Terminal();

t.clearScreen();
drawNumber(123456789, t);

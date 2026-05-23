import { ROWS, COLS } from "./settings.js";

export function isOpened(cell) {
  // Cell is opened if the cell class name does not contain "opened"
  return cell.className.indexOf("opened") !== -1;
}

export function isMarked(cell) {
  // Cell is marked if the cell class name contains "marked"
  return cell.className.indexOf("marked") !== -1;
}

export function executeForSurroundingCells(row, col, callback) {
  // Execute callback for all cells surrounding the cell at (row, col).
  // Does NOT execute callback for the cell at (row, col).
  for (let i = row - 1; i <= row + 1; i++) {
    for (let j = col - 1; j <= col + 1; j++) {
      if (i >= 0 && i < ROWS && j >= 0 && j < COLS && (i !== row || j !== col)) {
        callback(i, j);
      }
    }
  }
}

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
  for (let i = -1; i <= 1; i++) {
    for (let j = -1; j <= 1; j++) {
      if (!(i === 0 && j === 0) && row + i >= 0 && row + i < ROWS && col + j >= 0 && col + j < COLS) {
        const cell = app.childNodes[(row + i) * COLS + col + j];
        callback(cell, row + i, col + j);
      }
    }
  }
}

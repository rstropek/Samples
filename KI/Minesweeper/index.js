// Implement a simple Minesweeper game

import { isMarked, isOpened, executeForSurroundingCells } from "./helpers.js";
import { ROWS, COLS, BOMBS } from "./settings.js";

const app = document.querySelector("#app");
const gameStatus = document.querySelector("#game-status");

// 2d array containing flags indicating whether a cell is a bomb
const bombs = Array.from({ length: ROWS }, () => Array(COLS).fill(false));

// 2d array containing the number of bombs in surrounding cells
const counts = Array.from({ length: ROWS }, () => Array(COLS).fill(0));

// Create all cells
for (let row = 0; row < ROWS; row++) {
  for (let col = 0; col < COLS; col++) {
    const cell = document.createElement("div");
    cell.className = "cell";

    // Left click to open
    cell.addEventListener("click", () => {
      if (gameStatus.textContent) {
        return;
      }

      open(cell, row, col);

      if (hasWon()) {
        gameStatus.textContent = "You Win!";
      }
    });

    // Rick click to mark
    cell.addEventListener("contextmenu", (event) => {
      event.preventDefault();

      if (isOpened(cell)) {
        return;
      }

      if (isMarked(cell)) {
        cell.className = "cell";
      } else {
        cell.className = "cell marked";
      }
    });

    app.appendChild(cell);
  }
}

// Place bombs randomly
let numberOfBombs = 0;
while (numberOfBombs < BOMBS) {
  const row = Math.floor(Math.random() * ROWS);
  const col = Math.floor(Math.random() * COLS);
  if (!bombs[row][col]) {
    bombs[row][col] = true;
    numberOfBombs++;

    // Add bomb to surrounding cells
    executeForSurroundingCells(row, col, (i, j) => {
      counts[i][j]++;
    });
  }
}

function open(cell, row, col) {
  if (isOpened(cell) || isMarked(cell)) {
    return;
  }

  cell.className = "cell opened";

  if (bombs[row][col]) {
    gameStatus.textContent = "Game Over";
    cell.className = "cell exploded";
    return;
  }

  if (counts[row][col] === 0) {
    executeForSurroundingCells(row, col, (i, j) => {
      open(app.children[i * COLS + j], i, j);
    });
  } else {
    cell.textContent = counts[row][col];
  }
}

function hasWon() {
  // Count number of unopened cells. If the number
  // of unopened cells is equal to the number of bombs,
  // the player has won.
  let unopenedCells = 0;

  for (let row = 0; row < ROWS; row++) {
    for (let col = 0; col < COLS; col++) {
      const cell = app.children[row * COLS + col];
      if (!isOpened(cell)) {
        unopenedCells++;
      }
    }
  }

  return unopenedCells === BOMBS;
}
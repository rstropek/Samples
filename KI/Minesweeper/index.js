// Implement a simple Minesweeper game

import { isMarked, isOpened, executeForSurroundingCells } from "./helpers.js";
import { ROWS, COLS, BOMBS } from "./settings.js";

const app = document.querySelector("#app");
const gameStatus = document.querySelector("#game-status");

const bombs = []; // 2D array with flags indicating if a cell contains a bomb
const numberOfSurroundingBombs = []; // 2D array with the number of bombs surrounding a cell

// Create all cells
for (let row = 0; row < ROWS; row++) {
  bombs.push(new Array(COLS).fill(false));
  numberOfSurroundingBombs.push(new Array(COLS).fill(0));
  for (let col = 0; col < COLS; col++) {
    const cell = document.createElement("div");
    cell.className = "cell";

    cell.addEventListener("click", () => {
      // If the player has won or the cell has already been marked, do nothing
      if (hasWon() || isMarked(cell)) {
        return;
      }

      // If the cell contains a bomb, the game is over
      if (bombs[row][col]) {
        gameStatus.innerText = "Game Over";
        return;
      }

      // Open the cell
      open(cell, row, col);
      if (hasWon()) {
        gameStatus.innerText = "You won! 🥳";
      }
    });

    cell.addEventListener("contextmenu", (e) => {
      // Prevent the context menu from appearing
      e.preventDefault();

      // If the player has won or the cell has already been opened, do nothing
      if (hasWon() || isOpened(cell)) {
        return;
      }

      // Toggle the cell as marked
      cell.className = !isMarked(cell) ? "cell marked" : "cell";
    });
    app.appendChild(cell);
  }
}

// Place bombs randomly
let bombCount = 0;
while (bombCount != BOMBS) {
  const row = Math.floor(Math.random() * ROWS);
  const col = Math.floor(Math.random() * COLS);

  if (!bombs[row][col]) {
    bombs[row][col] = true;
    bombCount++;

    // Add bomb to number of surrounding cells
    executeForSurroundingCells(row, col, (_, row, col) => {
      numberOfSurroundingBombs[row][col]++;
    });
  }
}

function open(cell, row, col) {
  cell.className = "cell opened";
  if (numberOfSurroundingBombs[row][col] > 0) {
    cell.textContent = numberOfSurroundingBombs[row][col];
  } else {
    // Open surrounding cells recursively until cells with bombs nearby are found
    executeForSurroundingCells(row, col, (cell, row, col) => {
      if (!isOpened(cell) && !isMarked(cell)) {
        open(cell, row, col);
      }
    });
  }
}

function hasWon() {
  // The player has won if all only cells with bombs remain unopened.
  // Start by counting unopened cells
  let unopenedCount = 0;
  for (let row = 0; row < ROWS; row++) {
    for (let col = 0; col < COLS; col++) {
      if (!isOpened(app.childNodes[row * COLS + col])) {
        unopenedCount++;
      }
    }
  }

  // Check if the number of unopened cells is equal to the number of bombs
  return unopenedCount === BOMBS;
}

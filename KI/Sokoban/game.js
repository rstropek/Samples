import { GRID_CELL_SIZE } from './settings.js';
import { levels } from './levels.js';

const Wall = 0;
const Floor = 1;
const Target = 2;
const Box = 3;
const BoxOnTarget = 4;

const blockImageNames = [
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Blocks/block_06.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Ground/ground_01.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Ground/ground_04.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Crates/crate_43.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Crates/crate_08.png"
];

const playerImageNames = [
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Player/player_14.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Player/player_02.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Player/player_17.png",
  "https://cddataexchange.blob.core.windows.net/data-exchange/sokoban/Player/player_05.png"
];

export class SokobanGame {
  level;
  gameContent;
  player;
  playerPosition;
  onwin;
  winResolve;

  constructor(levelNumber) {
    this.level = [...levels[levelNumber]];
    this.gameContent = document.getElementById("game-content");
    this.player = document.getElementById('player');
    this.playerPos = this.findPlayer();
    this.buildLevelView();
  }

  buildLevelView() {
    for (const item of this.gameContent.children) {
      if (item.id !== "player") {
        this.gameContent.removeChild(item);
      }
    }

    let y = 0;
    for (const line of this.level) {
      let x = 0;
      for (const cell of line) {
        if (cell !== "_") {
          const image = this.getBlockImageBySymbol(cell);
          const block = document.createElement("div");
          block.className = "cell";
          block.style.transform = `translate(${x * 64}px, ${y * 64}px)`;
          block.style.backgroundImage = `url(${image})`;
          this.gameContent.insertBefore(block, player);
        }

        x++;
      }

      y++;
    }

  }

  findPlayer() {
    let y = 0;
    for (const line of this.level) {
      let x = 0;
      for (const cell of line) {
        if (cell === "@") {
          this.level[y] = this.replaceChar(this.level[y], " ", x);
          return { x, y };
        }

        x++;
      }

      y++;
    }
  }

  get width() {
    return this.level[0].length;
  }

  get height() {
    return this.level.length;
  }

  replaceChar(origString, replaceChar, index) {
    let firstPart = origString.substring(0, index);
    let lastPart = origString.substring(index + 1);

    let newString = firstPart + replaceChar + lastPart;
    return newString;
  }

  getBlockImageBySymbol(type) {
    switch (type) {
      case "X":
        return blockImageNames[Wall];
      case " ":
        return blockImageNames[Floor];
      case "@":
        return blockImageNames[Floor];
      case ".":
        return blockImageNames[Target];
      case "b":
        return blockImageNames[Box];
      case "B":
        return blockImageNames[BoxOnTarget];
    }
  }

  handleKeyboardEvent(e) {
    let image;
    let movement = { x: 0, y: 0 };
    if (e.key === 'ArrowLeft' && this.playerPosition.x > 0) {
      movement.x -= 1;
      image = playerImageNames[0];
    } else if (e.key === 'ArrowRight' && this.playerPosition.x < this.width) {
      movement.x += 1;
      image = playerImageNames[2];
    } else if (e.key === 'ArrowUp' && this.playerPosition.y > 0) {
      movement.y -= 1;
      image = playerImageNames[1];
    } else if (e.key === 'ArrowDown' && this.playerPosition.y < this.height) {
      movement.y += 1;
      image = playerImageNames[3];
    }

    if (image) {
      this.player.style.backgroundImage = `url(${image})`;
    }

    const target = { x: this.playerPosition.x + movement.x, y: this.playerPosition.y + movement.y };
    const targetBlock = this.level[target.y][target.x];
    if (targetBlock === "X") return;
    if (targetBlock === "b" || targetBlock === "B") {
      const nextAfterTarget = {
        x: target.x + movement.x,
        y: target.y + movement.y
      };
      const nextBlockAfterTarget = this.level[nextAfterTarget.y][nextAfterTarget.x];
      if (nextBlockAfterTarget !== " " && nextBlockAfterTarget !== ".") return;
      let targetBox;

      if (nextBlockAfterTarget === " ") {
        targetBox = "b";
      } else if (nextBlockAfterTarget === ".") {
        targetBox = "B";
      }

      this.level[nextAfterTarget.y] = this.replaceChar(
        this.level[nextAfterTarget.y],
        targetBox,
        nextAfterTarget.x
      );

      if (targetBlock === "B") {
        targetBox = ".";
      } else {
        targetBox = " ";
      }

      this.level[target.y] = this.replaceChar(this.level[target.y], targetBox, target.x);

      if (this.checkWin()) {
        this.winResolve();
      }
    }

    this.buildLevelView();

    this.playerPosition.x += movement.x;
    this.playerPosition.y += movement.y;

    this.player.style.transform = `translate(${this.playerPosition.x * GRID_CELL_SIZE}px, ${this.playerPosition.y * GRID_CELL_SIZE}px)`;
  }

  set playerPos(pos) {
    this.playerPosition = pos;
    this.player.style.transform = `translate(${this.playerPosition.x * GRID_CELL_SIZE}px, ${this.playerPosition.y * GRID_CELL_SIZE}px)`;
  }

  play() {
    return new Promise((res) => {
      this.winResolve = res;
    });
  }

  checkWin() {
    for (const line of this.level) {
      for (const cell of line) {
        if (cell === "b") return false;
      }
    }

    return true;
  }
}
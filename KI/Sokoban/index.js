import { SokobanGame } from "./game.js";
import { getNumberOfLevels } from "./levels.js";

let level = 0;
while (level < getNumberOfLevels()) {
  const game = new SokobanGame(level);
  document.onkeydown = (e) => game.handleKeyboardEvent(e);
  await game.play();
  level++;
}

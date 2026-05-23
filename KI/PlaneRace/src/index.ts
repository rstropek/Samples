import "./index.css";
import { Plane } from "./plane";
import { Player } from "./player";
import { Game } from "./game";

async function initGame() {
  // Create and load planes
  const plane1 = new Plane("plane1");
  const plane2 = new Plane("plane2");
  await Promise.all([plane1.load(), plane2.load()]);

  // Create game instance
  const game = new Game("canvas");

  // Create players
  const player1 = new Player(plane1, "d", 50);
  const player2 = new Player(plane2, "l", 350);

  // Add players to game
  game.addPlayer(player1);
  game.addPlayer(player2);

  // Start the game
  game.start();
}

// Initialize the game
initGame();

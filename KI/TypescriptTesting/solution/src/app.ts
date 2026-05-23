import { GuessingLogic } from './guessingLogic.js';
import { MasterMindConsole } from './masterMindConsole.js';
import { Game } from './game.js';

async function main() {
    const guessingLogic = new GuessingLogic();
    const console = new MasterMindConsole();
    const game = new Game(guessingLogic, console);
    
    await game.play();
}

main().catch(console.error);

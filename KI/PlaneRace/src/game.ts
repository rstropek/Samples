import { Player } from "./player";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private players: Player[] = [];
  private running: boolean = false;

  constructor(canvasId: string) {
    this.canvas = document.getElementById(canvasId) as HTMLCanvasElement;
    this.ctx = this.canvas.getContext("2d")!;

    // Set up canvas dimensions
    this.resizeCanvas();

    // Set up event listeners
    window.addEventListener("resize", () => this.resizeCanvas());
    document.addEventListener("keydown", (event) => {
      if (!event.repeat) {
        this.handleKeyPress(event.key, performance.now());
      }
    });
  }

  public addPlayer(player: Player): void {
    this.players.push(player);
  }

  private handleKeyPress(key: string, timestamp: number): void {
    for (const player of this.players) {
      player.handleKeyPress(key, timestamp);
    }
  }

  private resizeCanvas(): void {
    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;
  }

  public start(): void {
    if (!this.running) {
      this.running = true;
      this.gameLoop();
    }
  }

  private gameLoop(): void {
    if (!this.running) return;

    // Clear canvas
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

    const currentTime = performance.now();

    // Update and draw each player
    for (const player of this.players) {
      player.update(currentTime, this.canvas.width);
      player.draw(this.ctx);
    }

    // Draw HUD
    this.drawHUD();

    // Continue game loop
    requestAnimationFrame(() => this.gameLoop());
  }

  private drawHUD(): void {
    this.ctx.fillStyle = "black";
    this.ctx.font = "16px Arial";

    // Display player stats
    this.players.forEach((player, index) => {
      this.ctx.fillText(
        `Player ${index + 1}: ${player.getPressesPerSecond().toFixed(1)} presses/second`,
        10,
        30 + index * 25
      );
    });

    // Display target rate
    this.ctx.fillText(`Target: 4 presses/second to maintain position`, 10, 30 + this.players.length * 25);
  }

  public stop(): void {
    this.running = false;
  }
}

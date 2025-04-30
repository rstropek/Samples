import { Plane } from "./plane";

export class Player {
  private keyPressHistory: number[] = [];
  private _x: number = 0;

  constructor(
    private plane: Plane,
    private controlKey: string,
    private y: number,
    private neutralPressRate: number = 4,
    private maxHistoryTime: number = 1000,
    private movementSpeed: number = 5
  ) {}

  public get x(): number {
    return this._x;
  }

  public handleKeyPress(key: string, timestamp: number): void {
    if (key.toLowerCase() === this.controlKey) {
      this.keyPressHistory.push(timestamp);
    }
  }

  public update(currentTime: number, canvasWidth: number): void {
    // Clean up old presses
    this.keyPressHistory = this.keyPressHistory.filter(
      (timestamp) => currentTime - timestamp < this.maxHistoryTime
    );

    // Calculate presses per second
    const pressesPerSecond = this.keyPressHistory.length * (1000 / this.maxHistoryTime);

    // Update position based on press rate
    if (pressesPerSecond > this.neutralPressRate) {
      // Moving right - slower movement to make it harder
      const speedMultiplier = (pressesPerSecond - this.neutralPressRate) / 8;
      this._x += this.movementSpeed * speedMultiplier;
    } else if (pressesPerSecond < this.neutralPressRate) {
      // Moving left
      const speedMultiplier = (this.neutralPressRate - pressesPerSecond) / 4;
      this._x -= this.movementSpeed * speedMultiplier;
    }

    // Keep within canvas bounds
    this._x = Math.max(0, Math.min(canvasWidth, this._x));
  }

  public draw(ctx: CanvasRenderingContext2D): void {
    ctx.save();
    ctx.translate(this._x, this.y);
    this.plane.draw(ctx, true);
    ctx.restore();
  }

  public getPressesPerSecond(): number {
    return this.keyPressHistory.length * (1000 / this.maxHistoryTime);
  }
}

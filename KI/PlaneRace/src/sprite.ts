import { Frame, Spritesheet } from "./spritesheet";

export class Sprite {
  private spritesheet: Spritesheet;
  private fpsValue = 25;
  private timestampLastFrame = 0;
  private currentFrame = 0;
  private frames?: Frame[];

  constructor(name: string) {
    this.spritesheet = new Spritesheet(name);
  }

  public async load(): Promise<void> {
    await this.spritesheet.load();
  }

  public get fps(): number {
    return this.fpsValue;
  }

  public set fps(value: number) {
    this.fpsValue = value;
  }

  public get width(): number {
    return this.frames?.[this.currentFrame]?.frame.w ?? 0;
  }

  public get height(): number {
    return this.frames?.[this.currentFrame]?.frame.h ?? 0;
  }

  public set animationName(value: string) {
    this.frames = this.spritesheet.getFrames(value);
    this.currentFrame = 0;
  }

  public draw(ctx: CanvasRenderingContext2D): void {
    if (!this.frames) {
      return;
    }

    const now = performance.now();

    if (now - this.timestampLastFrame > 1000 / this.fpsValue) {
      this.currentFrame = (this.currentFrame + 1) % this.frames.length;
      this.timestampLastFrame = now;
    }

    this.spritesheet.drawFrame(this.frames[this.currentFrame], ctx);
  }
}

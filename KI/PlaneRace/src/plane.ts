import { Sprite } from "./sprite";

export class Plane {
  private static readonly verticalOffsetThrust: Record<string, { x: number; y: number }> = {
    plane1: { x: 6, y: -48 },
    plane2: { x: 3, y: -46 },
    plane3: { x: 44, y: 15 },
    plane4: { x: 87, y: -43 },
    plane5: { x: 57, y: 10 },
  };

  private spritesheet: Sprite;
  private thrust: Sprite;

  constructor(private name: string) {
    this.spritesheet = new Sprite(name);
    this.thrust = new Sprite("turbine");
  }

  public async load(): Promise<void> {
    await Promise.all([this.spritesheet.load(), this.thrust.load()]);
    this.spritesheet.animationName = "skeleton-MovingNIdle";
    this.thrust.animationName = "skeleton-animation";
  }

  public draw(ctx: CanvasRenderingContext2D, withThrust: boolean = true): void {
    this.spritesheet.draw(ctx);
    if (withThrust) {
      ctx.save();
      ctx.translate(-this.thrust.width + Plane.verticalOffsetThrust[this.name].x, Plane.verticalOffsetThrust[this.name].y);
      this.thrust.draw(ctx);
      ctx.restore();
    }
  }
}

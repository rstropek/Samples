import { ReadonlyVec2, vec2 } from 'gl-matrix';
import { random } from './random';
import { BubbleEdgeCollision, Direction, IMovableBubble } from './collision';

/**
 * Represents a bubble that moves around the canvas.
 */
export class Bubble implements IMovableBubble {
  center: vec2;
  velocity: ReadonlyVec2;
  radius: number;
  color: string;

  nextEdgeCollision: BubbleEdgeCollision | undefined;

  /**
   * Creates a new instance of `Bubble`.
   *
   * @param dimensions The dimensions of the canvas
   *
   * @remarks
   * The bubble is placed randomly on the canvas and moves in a random direction.
   */
  constructor(dimensions: { width: number; height: number }) {
    this.radius = random(20, 50);
    this.center = vec2.fromValues(
      random(this.radius, dimensions.width - this.radius),
      random(this.radius, dimensions.height - this.radius)
    );
    this.velocity = vec2.scale(vec2.create(), vec2.fromValues(random([-1, 1]), random([-1, 1])), random(200, 200));
    this.color = Bubble.getRandomColor();
  }

  private static getRandomColor(): string {
    const r = Math.floor(random(0, 256)); // Random between 0-255
    const g = Math.floor(random(0, 256)); // Random between 0-255
    const b = Math.floor(random(0, 256)); // Random between 0-255
    return `rgb(${r}, ${g}, ${b})`; // Return the color in rgb format
  }

  overlaps(other: Bubble): boolean {
    return vec2.distance(this.center, other.center) < this.radius + other.radius;
  }

  /**
   * Indicates whether the bubble is moving towards the given direction.
   *
   * @param direction The direction to check
   * @returns `true` if the bubble is moving towards the given direction; otherwise, `false`.
   */
  movesTowards(direction: Direction): boolean {
    switch (direction) {
      case Direction.West:
        return this.velocity[0] < 0;
      case Direction.East:
        return this.velocity[0] > 0;
      case Direction.North:
        return this.velocity[1] < 0;
      case Direction.South:
        return this.velocity[1] > 0;
      default:
        return false;
    }
  }

  /**
   * Returns the velocity of the bubble towards the given direction.
   *
   * @param direction The direction to check
   * @returns The absolute value of the velocity of the bubble towards the given direction.
   */
  getVelocityTowards(direction: Direction): number {
    switch (direction) {
      case Direction.West:
      case Direction.East:
        return Math.abs(this.velocity[0]);
      case Direction.North:
      case Direction.South:
        return Math.abs(this.velocity[1]);
      default:
        return 0;
    }
  }

  /**
   * Draws the bubble on the canvas.
   *
   * @param ctx The rendering context of the canvas
   */
  draw(ctx: CanvasRenderingContext2D, bubbles?: Bubble[]) {
    ctx.strokeStyle = this.color;
    ctx.lineWidth = 5;
    ctx.beginPath();
    ctx.arc(this.center[0], this.center[1], this.radius, 0, 2 * Math.PI);
    ctx.stroke();

    if (this.nextEdgeCollision) {
      ctx.save();
      ctx.setLineDash([5, 5]);
      ctx.beginPath();
      const collisionLocation = vec2.add(vec2.create(), this.center, vec2.scale(vec2.create(), this.velocity, this.nextEdgeCollision.msUntil / 1000));
      ctx.arc(collisionLocation[0], collisionLocation[1], this.radius, 0, 2 * Math.PI);
      ctx.stroke();
      ctx.restore();
    }

    if (bubbles) {
      const collisionTimes: number[] = [];
      for (const bubble of bubbles) {
        if (bubble === this) {
          continue;
        }

        const collisionTime = this.calculateCollisionTime(bubble);
        if (collisionTime) {
          collisionTimes.push(collisionTime);
        }
      }

      for (const ct of collisionTimes) {
        ctx.save();
        ctx.setLineDash([5, 5]);
        ctx.beginPath();
        const collisionLocation = vec2.add(vec2.create(), this.center, vec2.scale(vec2.create(), this.velocity, ct));
        ctx.arc(collisionLocation[0], collisionLocation[1], this.radius, 0, 2 * Math.PI);
        ctx.stroke();
        ctx.restore();
      }
    }
  }

  /**
   * Moves the center of the bubble according to its velocity.
   *
   * @param dt The time in seconds since the last frame
   */
  moveCenter(dt: number) {
    const movement = vec2.scale(vec2.create(), this.velocity, dt);
    this.center = vec2.add(this.center, this.center, movement);
  }

  /**
   * Changes the direction of the bubble when it collides with an edge.
   *
   * @param direction The direction of the collision
   * @param vec The velocity of the bubble
   * @returns The new velocity of the bubble
   */
  changeDirectionOnCollision(direction: Direction, vec: ReadonlyVec2): vec2 {
    switch (direction) {
      case Direction.West:
      case Direction.East:
        return vec2.fromValues(-vec[0], vec[1]);
      case Direction.North:
      case Direction.South:
        return vec2.fromValues(vec[0], -vec[1]);
      default:
        throw new Error('Invalid collision location');
    }
  }

  /**
   * Moves the bubble according to its velocity.
   *
   * @param dimensions The dimensions of the canvas
   * @param currentFrame The time of the current frame
   * @param prevFrame The time of the previous frame; if not provided, the bubble does not move
   */
  move(
    dimensions: { width: number; height: number },
    currentFrame: DOMHighResTimeStamp,
    prevFrame?: DOMHighResTimeStamp
  ) {
    // If there is no previous frame, move is called for the first time.
    // Therefore, the bubble does not move.
    if (!prevFrame) {
      return;
    }

      let repeat: boolean;
      do {
        repeat = false;

        // Check when the next collision with an edge will happen.
        this.nextEdgeCollision ??= new BubbleEdgeCollision()
          .getNextCollision(Direction.West, this, this.center[0])
          .getNextCollision(Direction.East, this, dimensions.width - this.center[0])
          .getNextCollision(Direction.North, this, this.center[1])
          .getNextCollision(Direction.South, this, dimensions.height - this.center[1]);

        // Check if the collision would have happend within the time between the previous and current frame.
        if (this.nextEdgeCollision.happensWithinMs(currentFrame - prevFrame)) {
          // Move the center of the bubble to the time of the collision.
          this.moveCenter(this.nextEdgeCollision.msUntil / 1000);

          // Reverse the correct velocity component.
          this.velocity = this.changeDirectionOnCollision(this.nextEdgeCollision.location, this.velocity);

          // Move the time of the previous frame to the time of the collision.
          prevFrame += this.nextEdgeCollision.msUntil;

          // Repeat the process to check if another collision would have happened within the remaining time.
          repeat = true;
          delete this.nextEdgeCollision;
        } else {
          this.nextEdgeCollision.msUntil -= currentFrame - prevFrame;
        }

      } while (repeat);

      // Calculate the movement that the bubble did between the previous and current frame.
      const movement = vec2.scale(vec2.create(), this.velocity, (currentFrame - prevFrame) / 1000);

      // Move the center of the bubble according to the calculated movement.
      vec2.add(this.center, this.center, movement);
  }

  calculateCollisionTime(other: Bubble): number | null {
    const v_rel = vec2.fromValues(other.velocity[0] - this.velocity[0], other.velocity[1] - this.velocity[1]);
    const d_initial = vec2.fromValues(other.center[0] - this.center[0], other.center[1] - this.center[1]);
    const r_sum = this.radius + other.radius;

    const a = v_rel[0] ** 2 + v_rel[1] ** 2;
    const b = 2 * (d_initial[0] * v_rel[0] + d_initial[1] * v_rel[1]);
    const c = d_initial[0] ** 2 + d_initial[1] ** 2 - r_sum ** 2;

    const discriminant = b ** 2 - 4 * a * c;

    if (discriminant < 0) {
      // No real roots, bubbles do not collide
      return null;
    }

    const t1 = (-b + Math.sqrt(discriminant)) / (2 * a);
    const t2 = (-b - Math.sqrt(discriminant)) / (2 * a);

    // We're interested in the first time of collision, so we take the minimum positive time
    const times = [t1, t2].filter((t) => t >= 0);
    if (times.length === 0) {
      return null;
    }

    return Math.min(...times);
  }
}

export enum Direction {
  Nowhere,
  West,
  North,
  East,
  South,
}

export interface IMovableBubble {
  radius: number;
  getVelocityTowards(direction: Direction): number;
  movesTowards(direction: Direction): boolean;
}

/**
 * Represents a collision with the edge of the canvas.
 */
export class BubbleEdgeCollision {
  /**
   * Indicates in which direction the collision will happen.
   *
   * If the value is `Direction.Nowhere`, this object does not represent a collision.
   */
  location: Direction;

  /**
   * The time in milliseconds until the collision will happen.
   *
   * If location is `Direction.Nowhere`, this value is `Number.MAX_VALUE`.
   */
  msUntil: number;

  /**
   * Creates a new instance of `EdgeCollision`.
   *
   * @param location The direction of the collision.
   * @param msUntil The time in milliseconds until the collision will happen.
   */
  constructor(location: Direction = Direction.Nowhere, msUntil?: number) {
    this.location = location;
    this.msUntil = msUntil ?? Number.MAX_VALUE;
  }

  /**
   * Returns a new `EdgeCollision` object that represents the next collision that will happen.
   *
   * @param direction The direction to check
   * @param bubble The bubble that is moving
   * @param distance The distance of the center of the bubble to the edge in the given direction
   * @returns A new `EdgeCollision` object that represents the next collision that will happen.
   *
   * @remarks
   * This method returns the current object if the collision in the given direction
   * will happen later than the collision represented by the current object.
   * Otherwise, a new object is returned that represents the collision in the given direction.
   */
  getNextCollision(direction: Direction, bubble: IMovableBubble, distance: number): BubbleEdgeCollision {
    if (bubble.movesTowards(direction)) {
      const timeUntilCollision = ((distance - bubble.radius) * 1000) / bubble.getVelocityTowards(direction);
      if (timeUntilCollision < this.msUntil) {
        return new BubbleEdgeCollision(direction, timeUntilCollision);
      }
    }
    return this;
  }

  /**
   * Indicates whether the collision will happen within the given time.
   *
   * @param ms The time in milliseconds
   * @returns `true` if the collision will happen within the given time; otherwise, `false`.
   *
   * @remarks
   * If the location is `Direction.Nowhere`, this method always returns `false`.
   */
  happensWithinMs(ms: number): boolean {
    return this.location !== Direction.Nowhere && this.msUntil < ms;
  }
}


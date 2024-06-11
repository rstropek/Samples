import { Vector } from './vector';

/**
 * Class representing a bubble
 *
 * A bubble is a circle with a position, a radius, and a color.
 * The color is the stroke color. The inside of bubbles is always white.
 */
export class Bubble {
  constructor(public position: Vector, public velocity: Vector, public radius: number, public color: string) {}

  /**
   * Draw the bubble on the canvas
   */
  draw(context: CanvasRenderingContext2D) {
    context.beginPath();
    context.arc(this.position.x, this.position.y, this.radius, 0, 2 * Math.PI);
    context.lineWidth = 3; // Set the line width to make the stroke thicker
    context.strokeStyle = this.color;
    context.stroke();
    context.fillStyle = 'white';
    context.fill();
  }

  move() {
    this.position = this.position.add(this.velocity);
  }

  bounceFromEdges(width: number, height: number) {
    if (this.position.x - this.radius < 0 || this.position.x + this.radius > width) {
      this.velocity = new Vector(-this.velocity.x, this.velocity.y);
      while (this.position.x - this.radius < 0 || this.position.x + this.radius > width) {
        this.move();
      }
    }
    if (this.position.y - this.radius < 0 || this.position.y + this.radius > height) {
      this.velocity = new Vector(this.velocity.x, -this.velocity.y);
      while (this.position.y - this.radius < 0 || this.position.y + this.radius > height) {
        this.move();
      }
    }
  }

  doCollide(other: Bubble) {
    const distanceVector = this.position.sub(other.position);
    const distance = distanceVector.mag();

    return distance <= this.radius + other.radius;
  }

  /**
   * Perform collision detection. If this bubble collides with the other bubble,
   * update the velocities of both bubbles.
   */
  collisionDetection(other: Bubble) {
    if (this.doCollide(other)) {
      const tempVelocity = this.velocity;
      this.velocity = other.velocity;
      other.velocity = tempVelocity;
    }
  }

  getCircumferencePoints(): Vector[] {
    const points: Vector[] = [];
    for (let angle = 0; angle < 360; angle += 10) {
      let x = this.position.x + this.radius * Math.cos((angle * Math.PI) / 180);
      let y = this.position.y + this.radius * Math.sin((angle * Math.PI) / 180);
      points.push(new Vector(x, y));
    }

    return points;
  }
}

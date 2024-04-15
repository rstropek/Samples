import { Vector } from './vector';

export class Polyline {
  points: Vector[] = [];

  constructor(points: Vector[] | undefined = undefined) {
    this.points = points ?? [];
  }

  clear() {
    this.points = [];
  }

  push(point: Vector) {
    this.points.push(point);
  }

  draw(context: CanvasRenderingContext2D) {
    if (this.points.length < 2) return;

    context.beginPath();
    context.moveTo(this.points[0].x, this.points[0].y);
    for (let i = 1; i < this.points.length; i++) {
      context.lineTo(this.points[i].x, this.points[i].y);
    }

    context.stroke();
  }

  doCross(other: Polyline): boolean {
    // Check each line segment of this polyline with each line segment of the other polyline
    for (let i = 0; i < this.points.length - 1; i++) {
      for (let j = 0; j < other.points.length - 1; j++) {
        if (
          Polyline.doIntersect(
            new Vector(this.points[i].x, this.points[i].y),
            new Vector(this.points[i + 1].x, this.points[i + 1].y),
            new Vector(other.points[j].x, other.points[j].y),
            new Vector(other.points[j + 1].x, other.points[j + 1].y)
          )
        ) {
          return true;
        }
      }
    }

    return false;
  }

  isClosed(): boolean {
    for (let i = 0; i < this.points.length - 1; i++) {
      for (let j = i + 2; j < this.points.length - 1; j++) {
        if (Polyline.doIntersect(this.points[i], this.points[i + 1], this.points[j], this.points[j + 1])) {
          return true;
        }
      }
    }
    return false;
  }

  isPointInside(point: Vector): boolean {
    if (!this.isClosed()) {
      return false;
    }

    let inside = false;
    for (let i = 0, j = this.points.length - 1; i < this.points.length; j = i++) {
      let xi = this.points[i].x,
        yi = this.points[i].y;
      let xj = this.points[j].x,
        yj = this.points[j].y;

      let intersect = yi > point.y != yj > point.y && point.x < ((xj - xi) * (point.y - yi)) / (yj - yi) + xi;
      if (intersect) inside = !inside;
    }
    return inside;
  }

  isFullyInside(other: Polyline): boolean {
    if (this.points.length == 0) {
      return false;
    }

    if (!this.isClosed()) {
      return false;
    }

    for (let i = 0; i < other.points.length; i++) {
      if (!this.isPointInside(other.points[i])) {
        return false;
      }
    }

    return true;
  }

  static doIntersect(p1: Vector, q1: Vector, p2: Vector, q2: Vector): boolean {
    // This function uses the orientation method to check for intersection
    // For explanation, see: https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    let o1 = Polyline.orientation(p1, q1, p2);
    let o2 = Polyline.orientation(p1, q1, q2);
    let o3 = Polyline.orientation(p2, q2, p1);
    let o4 = Polyline.orientation(p2, q2, q1);

    if (o1 != o2 && o3 != o4) {
      return true;
    }

    return false;
  }

  static orientation(p: Vector, q: Vector, r: Vector): number {
    let val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

    if (val == 0) return 0; // colinear

    return val > 0 ? 1 : 2; // clock or counterclock wise
  }
}

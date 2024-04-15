import { Bubble } from "./Bubble";
import "./style.css";
import { Vector } from "./vector";

// get reference to HTML canvas with id "game"
const canvas = document.getElementById("game") as HTMLCanvasElement;

const bubbles: Bubble[] = [];
let isDrawing = false;
let points: { x: number; y: number }[] = [];
let lost = false;
let win = false;

// Get the canvas context
// Get the device pixel ratio, falling back to 1.
var dpr = window.devicePixelRatio || 1;
// Get the size of the canvas in CSS pixels.
var rect = canvas.getBoundingClientRect();
canvas.width = rect.width * dpr;
canvas.height = rect.height * dpr;

fillBubbles(canvas.width, canvas.height, 10, 20, 20);
drawBubbles();

function fillBubbles(
  canvasWidth: number,
  canvasHeight: number,
  minRadius: number,
  maxRadius: number,
  numBubbles: number
) {
  // Fill bubbles with numBubbles random bubbles. Radius should be between minRadius and maxRadius.
  // Color should be 0..360 (HSB color space). Bubbles must not be partially
  // outside the canvas. Bubbles must not overlap.
  for (let i = 0; i < numBubbles; i++) {
    let radius = Math.random() * (maxRadius - minRadius) + minRadius;
    let x = Math.random() * (canvasWidth - 2 * radius) + radius;
    let y = Math.random() * (canvasHeight - 2 * radius) + radius;
    let color = Math.random() * 360;
    let velocity = new Vector(Math.random() * 2 - 1, Math.random() * 2 - 1);
    let bubble = new Bubble(
      new Vector(x, y),
      velocity,
      radius,
      `hsl(${color}, 100%, 50%)`
    );
    let overlapping = false;
    for (let j = 0; j < bubbles.length; j++) {
      let other = bubbles[j];
      let d = bubble.position.sub(other.position);
      let distance = d.mag();
      if (distance < bubble.radius + other.radius) {
        overlapping = true;
        break;
      }
    }
    if (!overlapping) {
      bubbles.push(bubble);
    } else {
      i--;
    }
  }
}

function drawBubbles() {
  // Draw all bubbles
  var context = canvas.getContext("2d")!;

  // Clear the canvas
  context.clearRect(0, 0, canvas.width, canvas.height);

  // If lost, draw "LOST" on the screen and exit
  if (lost || win) {
    context.fillStyle = "black";
    context.font = "30px Arial";
    context.fillText(lost ? "LOST" : "WIN", canvas.width / 2 - 30, canvas.height / 2);
    return;
  }

  drawPolygon(points);

  bubbles.forEach((bubble) => {
    bubble.move();

    if (doesBubbleIntersectPolyline(bubble, points)) {
      console.log("lost");
      
      lost = true;
      return;
    }

    for (let i = 0; i < bubbles.length - 1; i++) {
      for (let j = i + 1; j < bubbles.length; j++) {
        if (bubbles[i].doCollide(bubbles[j])) {
          bubbles[i].collisionDetection(bubbles[j]);
          while (bubbles[i].doCollide(bubbles[j])) {
            bubbles[i].move();
            bubbles[j].move();
          }
        }
      }
    }

    bubble.draw(context);
    bubble.bounceFromEdges(canvas.width, canvas.height);
  });
  requestAnimationFrame(drawBubbles);
}

canvas.addEventListener("mousedown", (event) => {
  isDrawing = true;
  points = []; // Clear the points
  points.push({ x: event.clientX, y: event.clientY });
});

canvas.addEventListener("mousemove", (event) => {
  if (!isDrawing) return;
  points.push({ x: event.clientX, y: event.clientY });
  if (isPolygonClosed()) {
    let bubblesInPolygon: Bubble[] = [];
    bubbles.forEach((bubble) => {
      let inOut = numberOfPointsInPolygon(bubble);
      if (inOut.inside > 0 && inOut.outside === 0) {
        bubblesInPolygon.push(bubble);
      }
    });

    // Remove all bubbles in bubbleInPolygon from bubbles
    bubblesInPolygon.forEach((b) => {
      let index = bubbles.indexOf(b);
      if (index > -1) {
        bubbles.splice(index, 1);
      }
    });

    if (bubbles.length === 0) {
      win = true;
    }

    isDrawing = false;
    points = [];
  }
});

canvas.addEventListener("mouseup", () => {
  isDrawing = false;
  points = [];
});

function drawPolygon(points: { x: number; y: number }[]) {
  if (points.length < 2) return;

  var context = canvas.getContext("2d")!;
  context.beginPath();
  context.moveTo(points[0].x, points[0].y);
  for (let i = 1; i < points.length; i++) {
    context.lineTo(points[i].x, points[i].y);
  }
  //context.closePath();
  context.stroke();
}

function doIntersect(
  p1: { x: number; y: number },
  q1: { x: number; y: number },
  p2: { x: number; y: number },
  q2: { x: number; y: number }
): boolean {
  // This function uses the orientation method to check for intersection
  // For explanation, see: https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
  let o1 = orientation(p1, q1, p2);
  let o2 = orientation(p1, q1, q2);
  let o3 = orientation(p2, q2, p1);
  let o4 = orientation(p2, q2, q1);

  if (o1 != o2 && o3 != o4) {
    return true;
  }

  return false;
}

function orientation(
  p: { x: number; y: number },
  q: { x: number; y: number },
  r: { x: number; y: number }
): number {
  let val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

  if (val == 0) return 0; // colinear

  return val > 0 ? 1 : 2; // clock or counterclock wise
}

function isPolygonClosed(): boolean {
  for (let i = 0; i < points.length - 1; i++) {
    for (let j = i + 2; j < points.length - 1; j++) {
      if (doIntersect(points[i], points[i + 1], points[j], points[j + 1])) {
        return true;
      }
    }
  }
  return false;
}

function isPointInPolygon(
  point: { x: number; y: number },
  polygon: { x: number; y: number }[]
): boolean {
  let inside = false;
  for (let i = 0, j = polygon.length - 1; i < polygon.length; j = i++) {
    let xi = polygon[i].x,
      yi = polygon[i].y;
    let xj = polygon[j].x,
      yj = polygon[j].y;

    let intersect =
      yi > point.y != yj > point.y &&
      point.x < ((xj - xi) * (point.y - yi)) / (yj - yi) + xi;
    if (intersect) inside = !inside;
  }
  return inside;
}

function numberOfPointsInPolygon(bubble: Bubble): {
  inside: number;
  outside: number;
} {
  let center = bubble.position;
  let inside = 0;
  let outside = 0;
  for (let angle = 0; angle < 360; angle += 10) {
    let x = center.x + bubble.radius * Math.cos((angle * Math.PI) / 180);
    let y = center.y + bubble.radius * Math.sin((angle * Math.PI) / 180);
    if (isPointInPolygon({ x, y }, points)) {
      inside++;
    } else {
      outside++;
    }
  }

  return { inside, outside };
}

function doesBubbleIntersectPolyline(bubble: Bubble, polyline: {x: number, y: number}[]): boolean {
  let center = bubble.position;
  for (let angle = 0; angle < 360; angle += 10) {
    let x = center.x + bubble.radius * Math.cos((angle * Math.PI) / 180);
    let y = center.y + bubble.radius * Math.sin((angle * Math.PI) / 180);
    let point1 = {x, y};
    x = center.x + bubble.radius * Math.cos(((angle + 10) * Math.PI) / 180);
    y = center.y + bubble.radius * Math.sin(((angle + 10) * Math.PI) / 180);
    let point2 = {x, y};
    for (let i = 0; i < polyline.length - 1; i++) {
      if (doIntersect(point1, point2, polyline[i], polyline[i + 1])) {
        return true;
      }
    }
  }
  return false;
}
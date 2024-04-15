import { Bubble } from './bubble';
import { Polyline } from './polyline';
import './style.css';
import { Vector } from './vector';

enum GameState {
  Running,
  Lost,
  Win,
}

// get reference to HTML canvas with id "game"
const canvas = document.getElementById('game') as HTMLCanvasElement;

const bubbles: Bubble[] = [];
const polyline = new Polyline();
let isDrawing = false;
let gameState: GameState = GameState.Running;

var dpr = window.devicePixelRatio || 1;
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
    let bubble = new Bubble(new Vector(x, y), velocity, radius, `hsl(${color}, 100%, 50%)`);
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
  var context = canvas.getContext('2d')!;

  // Clear the canvas
  context.clearRect(0, 0, canvas.width, canvas.height);

  // If lost or win, draw the corresponding message on the screen and exit
  if (gameState === GameState.Lost || gameState === GameState.Win) {
    context.fillStyle = 'black';
    context.font = '50px Arial';
    let text: string;
    switch (gameState) {
      case GameState.Lost:
        text = 'You lost!';
        break;
      case GameState.Win:
        text = 'You won!';
        break;
    }

    let metrics = context.measureText(text);
    let textWidth = metrics.width;
    let textHeight = 50; // This is the font size. Ideally, you should calculate this.

    let x = (canvas.width - textWidth) / 2;
    let y = (canvas.height - textHeight) / 2 + textHeight; // We add textHeight because the y position is from the bottom of the text

    context.fillText(text, x, y);
    return;
  }

  polyline.draw(context);

  bubbles.forEach((bubble) => {
    bubble.move();

    if (polyline.doCross(new Polyline(bubble.getCircumferencePoints()))) {
      gameState = GameState.Lost;
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

canvas.addEventListener('mousedown', (event) => {
  isDrawing = true;
  polyline.clear();
  polyline.push(new Vector(event.clientX, event.clientY));
});

canvas.addEventListener('mousemove', (event) => {
  if (!isDrawing) {
    return;
  }
  polyline.push(new Vector(event.clientX, event.clientY));
  if (polyline.isClosed()) {
    let bubblesInPolygon: Bubble[] = [];
    bubbles.forEach((bubble) => {
      if (polyline.isFullyInside(new Polyline(bubble.getCircumferencePoints()))) {
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
      gameState = GameState.Win;
    }

    isDrawing = false;
    polyline.clear();
  }
});

canvas.addEventListener('mouseup', () => {
  isDrawing = false;
  polyline.clear();
});

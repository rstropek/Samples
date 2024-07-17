import { Bubble } from './bubble';

const WIDTH = 1024;
const HEIGHT = 1024;

const canvas = document.getElementById('app') as HTMLCanvasElement;
canvas.width = WIDTH;
canvas.height = HEIGHT;

const ctx = canvas.getContext('2d')!;

const bubbles: Bubble[] = [];
while (bubbles.length < 3) {
  const newBubble = new Bubble({ width: canvas.width, height: canvas.height });
  let overlapping = false;
  for (const bubble of bubbles) {
    if (newBubble.overlaps(bubble)) {
      overlapping = true;
      break;
    }
  }

  if (!overlapping) {
    bubbles.push(newBubble);
  }
}

let lastFrameTime: DOMHighResTimeStamp | undefined;
draw();

async function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  const currentFrameTime = performance.now();
  bubbles.forEach(bubble => bubble.move({ width: canvas.width, height: canvas.height }, currentFrameTime, lastFrameTime));
  bubbles.forEach(bubble => bubble.draw(ctx, bubbles));
  lastFrameTime = currentFrameTime;
  requestAnimationFrame(draw);
}

// function delay(ms: number): Promise<void> {
//   return new Promise((resolve) => setTimeout(resolve, ms));
// }

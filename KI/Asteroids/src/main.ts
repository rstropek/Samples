import { AsteroidCanvas } from './drawing-context.js';
import './rocket-launcher.js';
import { getLaunchPosition, moveRocket, Rocket } from './rocket.js';

const canvas = document.getElementById('gameCanvas') as HTMLCanvasElement;

let angle = 0;
let isLeftDown = false;
let isRightDown = false;

const rockets: Rocket[] = [];

function draw() {
  const ctx = canvas.getContext('2d')!;
  ctx.fillStyle = 'black';
  ctx.fillRect(0, 0, canvas.width, canvas.height);

  if (isLeftDown) {
    angle -= 0.05;
  }
  if (isRightDown) {
    angle += 0.05;
  }

  ctx.save();
  try {
    // Set the origin to the bottom center of the canvas
    ctx.translate(canvas.width / 2, canvas.height);

    const asteroidCtx = new AsteroidCanvas(ctx);
    asteroidCtx.angle = angle;
    asteroidCtx.drawRocketLauncher();

    rockets.forEach((rocket) => {
      asteroidCtx.drawRocket(rocket);
      moveRocket(rocket);
    });
    asteroidCtx.removeOutOfBoundsRockets(rockets);
  } finally {
    ctx.restore();
  }

  requestAnimationFrame(draw);
}

document.addEventListener('keydown', (e) => {
  if (e.key === 'ArrowRight') {
    isRightDown = true;
  } else if (e.key === 'ArrowLeft') {
    isLeftDown = true;
  } else if (e.key === ' ') {
    rockets.push({ ...getLaunchPosition(angle), angle });
  }
});

document.addEventListener('keyup', (e) => {
  if (e.key === 'ArrowRight') {
    isRightDown = false;
  } else if (e.key === 'ArrowLeft') {
    isLeftDown = false;
  }
});

draw();

import { AsteroidCanvas } from './drawing-context.js';
import { DIAMETER, CANON_LENGTH } from './rocket-launcher.js';

export type Rocket = { x: number; y: number; angle: number };

declare module './drawing-context.js' {
  interface AsteroidCanvas {
    drawRocket(rocket: Rocket): void;
    removeOutOfBoundsRockets(rockets: Rocket[]): void;
  }
}

AsteroidCanvas.prototype.drawRocket = function (rocket: Rocket) {
  this.ctx.save();

  this.ctx.strokeStyle = 'yellow';
  this.ctx.lineWidth = 3;
  this.ctx.shadowBlur = 15;
  this.ctx.shadowColor = 'yellow';
  this.ctx.shadowOffsetX = 0;
  this.ctx.shadowOffsetY = 0;

  this.ctx.translate(rocket.x, -rocket.y);
  this.ctx.rotate(rocket.angle);
  this.ctx.beginPath();
  this.ctx.moveTo(0, 0);
  this.ctx.lineTo(0, -10);
  this.ctx.stroke();
  this.ctx.closePath();

  this.ctx.restore();
};

AsteroidCanvas.prototype.removeOutOfBoundsRockets = function (rockets: Rocket[]) {
  for (let i = rockets.length - 1; i >= 0; i--) {
    const rocket = rockets[i];
    if (
      rocket.x < -this.ctx.canvas.width / 2 ||
      rocket.x > this.ctx.canvas.width / 2 ||
      rocket.y > this.ctx.canvas.height
    ) {
      rockets.splice(i, 1);
    }
  }
};

export function getLaunchPosition(canonAngle: number): {
  x: number;
  y: number;
} {
  return {
    x: Math.sin(canonAngle) * (DIAMETER + CANON_LENGTH),
    y: Math.cos(canonAngle) * (DIAMETER + CANON_LENGTH),
  };
}

export function moveRocket(rocket: Rocket) {
  rocket.x += Math.sin(rocket.angle) * 2;
  rocket.y += Math.cos(rocket.angle) * 2;
}

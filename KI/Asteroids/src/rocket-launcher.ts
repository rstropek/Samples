import { AsteroidCanvas } from './drawing-context.js';

export const DIAMETER = 25;
export const CANON_LENGTH = 15;

declare module './drawing-context.js' {
  interface AsteroidCanvas {
    angle: number;
    drawRocketLauncher(): void;
  }
}

AsteroidCanvas.prototype.angle = 0;

AsteroidCanvas.prototype.drawRocketLauncher = function () {
  this.ctx.save();

  this.ctx.strokeStyle = 'yellow';
  this.ctx.lineWidth = 3;
  this.ctx.shadowBlur = 15;
  this.ctx.shadowColor = 'yellow';
  this.ctx.shadowOffsetX = 0;
  this.ctx.shadowOffsetY = 0;

  this.ctx.beginPath();
  this.ctx.arc(0, 0, DIAMETER, Math.PI, 2 * Math.PI);
  this.ctx.stroke();
  this.ctx.closePath();

  this.ctx.rotate(this.angle);
  this.ctx.beginPath();
  this.ctx.lineWidth = 5;
  this.ctx.moveTo(0, -DIAMETER);
  this.ctx.lineTo(0, -(DIAMETER + CANON_LENGTH));
  this.ctx.stroke();
  this.ctx.closePath();

  this.ctx.restore();
};

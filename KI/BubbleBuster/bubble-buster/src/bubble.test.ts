import { expect, test } from 'vitest';
import { Vector } from './vector';
import { Bubble } from './bubble';

test('Bubble: move', () => {
  const bubble = new Bubble(new Vector(0, 0), new Vector(1, 1), 10, 'red');
  bubble.move();
  expect(bubble.position.x).toBe(1);
  expect(bubble.position.y).toBe(1);
});

test('Bubble: bounceFromEdges', () => {
  const bubble = new Bubble(new Vector(11, 23), new Vector(1, 1), 10, 'red');
  bubble.bounceFromEdges(25, 25);
  expect(bubble.velocity.x).toBe(1);
  expect(bubble.velocity.y).toBe(-1);
  expect(bubble.position.y).toBeLessThanOrEqual(20)
});

test('Bubble: doCollide', () => {
  const bubble1 = new Bubble(new Vector(0, 0), new Vector(1, 1), 10, 'red');
  const bubble2 = new Bubble(new Vector(15, 15), new Vector(-1, -1), 10, 'blue');
  expect(bubble1.doCollide(bubble2)).toBe(false);

  const bubble3 = new Bubble(new Vector(0, 0), new Vector(1, 1), 10, 'red');
  const bubble4 = new Bubble(new Vector(5, 5), new Vector(-1, -1), 10, 'blue');
  expect(bubble3.doCollide(bubble4)).toBe(true);
});

test('Bubble: collisionDetection', () => {
  const bubble1 = new Bubble(new Vector(0, 0), new Vector(1, 1), 10, 'red');
  const bubble2 = new Bubble(new Vector(5, 5), new Vector(-1, -1), 10, 'blue');
  bubble1.collisionDetection(bubble2);
  expect(bubble1.velocity.x).toBe(-1);
  expect(bubble1.velocity.y).toBe(-1);
  expect(bubble2.velocity.x).toBe(1);
  expect(bubble2.velocity.y).toBe(1);
});

test('Bubble: getCircumferencePoints', () => {
  const bubble = new Bubble(new Vector(0, 0), new Vector(1, 1), 10, 'red');
  const points = bubble.getCircumferencePoints();

  // Check if the correct number of points is returned
  expect(points.length).toBe(36);

  // Check if all points are on the circumference of the bubble
  points.forEach((point) => {
    const distance = Math.sqrt(Math.pow(point.x - bubble.position.x, 2) + Math.pow(point.y - bubble.position.y, 2));
    expect(distance).toBeCloseTo(bubble.radius, 1); // Allow a small error due to rounding
  });
});

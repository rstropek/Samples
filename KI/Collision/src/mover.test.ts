import { describe, beforeEach, it, expect } from '@jest/globals';
import { Bubble } from './bubble';
import { vec2 } from 'gl-matrix';

describe('Bubble move method', () => {
  let bubble: Bubble;

  beforeEach(() => {
    bubble = new Bubble({ width: 100, height: 100 });
    // Initialize bubble properties as needed
  });

  it('does not move if prevFrame is not provided', () => {
    const initialCenter = vec2.clone(bubble.center);
    bubble.move({width: 100, height: 100}, 1000);
    expect(bubble.center).toEqual(initialCenter);
  });

  it('moves correctly without collisions', () => {
    const initialCenter = vec2.fromValues(500, 500);
    bubble.center = vec2.clone(initialCenter);
    bubble.radius = 10;
    bubble.velocity = vec2.fromValues(10, 0); // Move to the right
    bubble.move({width: 1000, height: 1000}, 2000, 1000); // Move for 10ms
    expect(bubble.center).toEqual(vec2.fromValues(500 + 10, 500)); // Moved right by 1 unit
  });


  it('correctly handles collision with an edge', () => {
    const initialCenter = vec2.fromValues(50, 500); // Close to the right edge
    bubble.center = vec2.clone(initialCenter);
    bubble.velocity = vec2.fromValues(-300, 0); // Moving to the left
    bubble.radius = 40;

    // Assuming the bubble has a radius that would cause it to collide with the right edge
    // when moving from 95 to beyond 100 (edge of the container) within 100ms.
    bubble.move({width: 1000, height: 1000}, 2000, 1000); // Move for 1s

    // After collision, the bubble should bounce back, reversing its x velocity.
    // The exact new position depends on the bubble's radius and the specifics of the collision response.
    // Here, we're checking that the bubble's x velocity has been reversed.
    expect(bubble.velocity[0]).toBeGreaterThan(0);

    // Additionally, check that the bubble's center is now less than the initial x position,
    // indicating it has bounced back.
    expect(bubble.center[0]).toBeGreaterThan(initialCenter[0]);
  });
});

import { describe, beforeEach, it, expect, jest } from '@jest/globals';
import { BubbleEdgeCollision, Direction } from './collision';
import { Bubble } from './bubble'; // Assuming Bubble class exists and has necessary methods

describe('BubbleEdgeCollision', () => {
  describe('constructor', () => {
    it('initializes with default values if no arguments are provided', () => {
      const collision = new BubbleEdgeCollision();
      expect(collision.location).toBe(Direction.Nowhere);
      expect(collision.msUntil).toBe(Number.MAX_VALUE);
    });

    it('initializes with provided values', () => {
      const collision = new BubbleEdgeCollision(Direction.North, 5000);
      expect(collision.location).toBe(Direction.North);
      expect(collision.msUntil).toBe(5000);
    });
  });

  describe('getNextCollision', () => {
    let bubbleMock: Bubble;

    beforeEach(() => {
      bubbleMock = {
        radius: 10,
        getVelocityTowards: jest.fn().mockReturnValue(100),
        movesTowards: jest.fn().mockReturnValue(true),
      } as unknown as Bubble;
    });

    it('returns a new collision if the next collision happens sooner', () => {
      const currentCollision = new BubbleEdgeCollision(Direction.West, 2000);
      const nextCollision = currentCollision.getNextCollision(Direction.North, bubbleMock, 150);
      expect(nextCollision).not.toBe(currentCollision);
      expect(nextCollision.msUntil).toBeLessThan(currentCollision.msUntil);
    });

    it('returns the same object if the next collision does not happen sooner', () => {
      const currentCollision = new BubbleEdgeCollision(Direction.West, 1000);
      const nextCollision = currentCollision.getNextCollision(Direction.North, bubbleMock, 150);
      expect(nextCollision).toBe(currentCollision);
    });

    it('returns the same object if the bubble is not moving towards the given direction', () => {
      jest.spyOn(bubbleMock, 'movesTowards').mockReturnValueOnce(false);
      const currentCollision = new BubbleEdgeCollision(Direction.West, 1000);
      const nextCollision = currentCollision.getNextCollision(Direction.North, bubbleMock, 150);
      expect(nextCollision).toBe(currentCollision);
    });
  });

  describe('happensWithinMs', () => {
    it('returns true if the collision happens within the given time', () => {
      const collision = new BubbleEdgeCollision(Direction.North, 500);
      expect(collision.happensWithinMs(1000)).toBeTruthy();
    });

    it('returns false if the collision does not happen within the given time', () => {
      const collision = new BubbleEdgeCollision(Direction.North, 1500);
      expect(collision.happensWithinMs(1000)).toBeFalsy();
    });

    it('returns false if the location is Direction.Nowhere', () => {
      const collision = new BubbleEdgeCollision(Direction.Nowhere, Number.MAX_VALUE);
      expect(collision.happensWithinMs(1000)).toBeFalsy();
    });
  });
});

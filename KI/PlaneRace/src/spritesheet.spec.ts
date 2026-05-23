import {describe, expect, test} from '@jest/globals';
import { Spritesheet, Frame } from './spritesheet';

// Mock fetch globally
global.fetch = jest.fn();

// Mock Image
global.Image = class {
  onload: () => void = () => {};
  src: string = '';

  constructor() {
    // Automatically trigger onload when src is set
    setTimeout(() => {
      if (this.onload) this.onload();
    }, 0);
  }
} as any;

describe('Spritesheet', () => {
  let spritesheet: Spritesheet;
  let mockContext: CanvasRenderingContext2D;

  const mockFrames: Frame[] = [
    {
      filename: 'walk_0',
      frame: { x: 0, y: 0, w: 32, h: 32 }
    },
    {
      filename: 'walk_2',
      frame: { x: 64, y: 0, w: 32, h: 32 }
    },
    {
      filename: 'walk_01',
      frame: { x: 32, y: 0, w: 32, h: 32 }
    },
    {
      filename: 'idle_0',
      frame: { x: 64, y: 0, w: 32, h: 32 }
    }
  ];

  beforeEach(() => {
    // Reset mocks
    jest.clearAllMocks();

    // Mock fetch response
    (global.fetch as jest.Mock).mockResolvedValue({
      json: jest.fn().mockResolvedValue({
        frames: mockFrames
      })
    });

    // Create spritesheet
    spritesheet = new Spritesheet('character');

    // Mock canvas context
    mockContext = {
      drawImage: jest.fn()
    } as unknown as CanvasRenderingContext2D;
  });

  describe('getFrames', () => {
    it('should throw error if spritesheet not loaded', () => {
      expect(() => {
        spritesheet.getFrames('walk');
      }).toThrow('Spritesheet not loaded');
    });

    it('should filter and sort frames by animation name', async () => {
      // Load the spritesheet
      await spritesheet.load();

      // Get walk frames
      const walkFrames = spritesheet.getFrames('walk');

      // Should return 2 frames
      expect(walkFrames.length).toBe(3);
      expect(walkFrames[0].filename).toBe('walk_0');
      expect(walkFrames[1].filename).toBe('walk_01');
      expect(walkFrames[2].filename).toBe('walk_2');
    });

    it('should return empty array if no matching frames', async () => {
      await spritesheet.load();

      const runFrames = spritesheet.getFrames('run');

      expect(runFrames.length).toBe(0);
    });
  });

  describe('drawFrame', () => {
    it('should throw error if spritesheet not loaded', () => {
      const frame = mockFrames[0];

      expect(() => {
        spritesheet.drawFrame(frame, mockContext);
      }).toThrow('Spritesheet not loaded');
    });

    it('should draw the frame with correct parameters', async () => {
      // Load the spritesheet
      await spritesheet.load();

      // Get a frame
      const frame = mockFrames[0];

      // Draw the frame
      spritesheet.drawFrame(frame, mockContext);

      // Check if drawImage was called with correct parameters
      expect(mockContext.drawImage).toHaveBeenCalledTimes(1);
      expect(mockContext.drawImage).toHaveBeenCalledWith(
        expect.any(Image),
        frame.frame.x,
        frame.frame.y,
        frame.frame.w,
        frame.frame.h,
        0,
        0,
        frame.frame.w,
        frame.frame.h
      );
    });
  });
});
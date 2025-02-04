import { degreeToRadian } from './math_helper';

describe('math_helper', () => {
  describe('degreeToRadian', () => {
    it('should convert degrees to radians correctly', () => {
      expect(degreeToRadian(180)).toBeCloseTo(Math.PI);
      expect(degreeToRadian(90)).toBeCloseTo(Math.PI / 2);
      expect(degreeToRadian(0)).toBeCloseTo(0);
    });
  });
});

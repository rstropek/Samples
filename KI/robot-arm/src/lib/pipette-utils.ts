/**
 * Calculates the required pipette tilt angle to keep the pipette vertical (pointing downwards)
 * given the current joint angles of the robot arm.
 * 
 * @param j1 Shoulder pitch angle in degrees
 * @param j2 Elbow pitch angle in degrees  
 * @param j3 Wrist pitch angle in degrees
 * @returns The required pipette tilt angle in degrees to maintain vertical orientation
 */
export function calculateVerticalPipetteTilt(j1: number, j2: number, j3: number): number {
  // The total pitch accumulated through the arm chain needs to be compensated
  // J1 has negative rotation for upward movement, so we need to account for that
  // The total pitch rotation is: -j1 + j2 + j3
  // To keep the pipette vertical (pointing down), we need to compensate this rotation
  const totalPitch = -j1 + j2 + j3;
  
  // The pipette tilt should counteract the accumulated pitch to maintain vertical orientation
  return -totalPitch;
}

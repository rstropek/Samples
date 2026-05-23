'use client';

import { useState } from "react";
import Image from "next/image";
import styles from "./page.module.css";
import ThreeScene from "@/components/arm";
import Slider, { RobotArmSliders, JointValues } from "@/components/slider";

export default function Home() {
  const [jointValues, setJointValues] = useState<JointValues>({
    j0: 0,   // yaw: -180 to 180, default 0
    j1: 75,  // pitch: 0-90, default 75
    j2: 45,  // pitch: -90 to 90, default 45
    j3: 15,  // pitch: -90 to 90, default 15
    j4: 0    // pitch: calculated automatically for vertical pipette
  });

  return (
    <>
      <RobotArmSliders 
        values={jointValues}
        onChange={setJointValues}
      />
      <ThreeScene 
        className="scene" 
        jointValues={jointValues}
      />
    </>
  );
}

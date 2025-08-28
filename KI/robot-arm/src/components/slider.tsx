'use client';

import { useState } from 'react';
import styles from './slider.module.css';

interface SliderProps {
  label: string;
  min: number;
  max: number;
  step?: number;
  defaultValue?: number;
  value?: number;
  onChange?: (value: number) => void;
  disabled?: boolean;
  className?: string;
}

export default function Slider({
  label,
  min,
  max,
  step = 1,
  defaultValue,
  value: controlledValue,
  onChange,
  disabled = false,
  className = ''
}: SliderProps) {
  const [internalValue, setInternalValue] = useState(defaultValue ?? min);
  
  // Use controlled value if provided, otherwise use internal state
  const currentValue = controlledValue !== undefined ? controlledValue : internalValue;
  
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = parseFloat(event.target.value);
    
    if (controlledValue === undefined) {
      setInternalValue(newValue);
    }
    
    onChange?.(newValue);
  };

  return (
    <div className={`${styles.sliderContainer} ${className}`}>
      <label className={styles.label} htmlFor={`slider-${label}`}>
        {label}: {currentValue}
      </label>
      <div className={styles.sliderWrapper}>
        <span className={styles.minValue}>{min}</span>
        <input
          id={`slider-${label}`}
          type="range"
          min={min}
          max={max}
          step={step}
          value={currentValue}
          onChange={handleChange}
          disabled={disabled}
          className={styles.slider}
        />
        <span className={styles.maxValue}>{max}</span>
      </div>
    </div>
  );
}

// Robot arm joint values interface
export interface JointValues {
  j0: number; // yaw
  j1: number; // pitch
  j2: number; // pitch
  j3: number; // pitch
  j4: number; // pitch (calculated automatically for vertical pipette)
}

interface RobotArmSlidersProps {
  values?: JointValues;
  onChange?: (values: JointValues) => void;
  disabled?: boolean;
  className?: string;
}

export function RobotArmSliders({
  values,
  onChange,
  disabled = false,
  className = ''
}: RobotArmSlidersProps) {
  const [internalValues, setInternalValues] = useState<JointValues>({
    j0: 0,   // yaw: -180 to 180, default 0
    j1: 75,  // pitch: 0-90, default 75
    j2: 45,  // pitch: -90 to 90, default 45
    j3: 15,  // pitch: -90 to 90, default 15
    j4: 0    // pitch: calculated automatically for vertical pipette
  });

  // Use controlled values if provided, otherwise use internal state
  const currentValues = values || internalValues;

  const handleSliderChange = (joint: keyof JointValues) => (value: number) => {
    const newValues = { ...currentValues, [joint]: value };
    
    if (!values) {
      setInternalValues(newValues);
    }
    
    onChange?.(newValues);
  };

  return (
    <div className={`${styles.robotArmContainer} ${className}`}>
      <Slider
        label="J0 (base yaw)"
        min={-180}
        max={180}
        step={1}
        value={currentValues.j0}
        onChange={handleSliderChange('j0')}
        disabled={disabled}
      />
      <Slider
        label="J1 (shoulder pitch)"
        min={0}
        max={90}
        step={1}
        value={currentValues.j1}
        onChange={handleSliderChange('j1')}
        disabled={disabled}
      />
      <Slider
        label="J2 (elbow pitch)"
        min={-90}
        max={90}
        step={1}
        value={currentValues.j2}
        onChange={handleSliderChange('j2')}
        disabled={disabled}
      />
      <Slider
        label="J3 (wrist pitch)"
        min={-90}
        max={90}
        step={1}
        value={currentValues.j3}
        onChange={handleSliderChange('j3')}
        disabled={disabled}
      />
    </div>
  );
}
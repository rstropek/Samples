# Pipette always vertical

This is a Next.js application that simulates a robotic arm with a pipette. It uses three.js for 3D visualization.

Before doing any work, read the following files as they are relevant for your task:

* [arm.tsx](../../src/components/arm.tsx): Robot arm visualization
* [slider.tsx](../../src/components/slider.tsx): Slider components for joint control
* [page.tsx](../../src/app/page.tsx): Main application page

## Requirements

Currently, all joints can be controlled by sliders.

Change the implementation as follows:

* Remove the manual control of the pipette tilt
* Add a helper function to the `lib` folder that calculates the required tilt so that the pipette remains vertical pointing downwards.
* Update the arm.tsx file to use the new helper function

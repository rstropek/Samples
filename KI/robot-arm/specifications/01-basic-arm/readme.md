# Draw 4-axis robot arm in home position (Next.js + Three.js)

## Current State

This is an existing, working Next.js 15 app with Three.js installed. There is an existing, working demo component with three.js in [arm.tsx](../../src/components/arm.tsx) that is already embedded in [page.tsx](../../src/app/page.tsx).

## Goal

Render a static scene showing a 5-axis robot arm at a **meaningful home pose** with correct dimensions and simple geometry:

* Base (cylinder): **Ø=50 cm, H=20 cm**
* Segment 1 (cuboid, shoulder link): **10×10×60 cm**
* Segment 2 (cuboid, elbow link): **8×8×45 cm**
* Segment 3 (cuboid, wrist link): **6×6×25 cm**
* Segment 4 (cylinder, pipette): **Ø=2 cm, H=10 cm**
* Joints: spheres
    * Joint sphere radii: shoulder 6cm, elbow 5cm, wrist 4cm, pipette 1.5cm
* Camera positioned for a clear view; basic lighting; ground grid.

## Requirements

* No new dependencies
* Use **meters** internally (1 cm = 0.01 m).
* Keep everything related to the scene in _arm.tsx_. Do not create new files for now.
* Define segment dimensions in constants at the beginning of the file.
* In the future, we want to make the joints (yaw/pitch) changeable from outside. Do NOT implement this feature yet, but write code that makes it easy to add this feature later.

## Scene & coordinate system

* **World axes:** Three.js default (Y up).
* **Ground:** grid on the XZ plane at `y=0`.
* **Base origin:** center of the base cylinder sits so its **bottom** rests on ground. (i.e., base center at `y = baseHeight/2`.)

## Home pose (angles)

A “pleasant” display pose that looks naturally folded and clear of the base.

* Joint order: **J0 (base yaw), J1 (shoulder pitch), J2 (elbow pitch), J3 (wrist pitch), J4 (pipette tilt)**
* **J0 (yaw around world Y):** `0°` (looking along +Z)
* **J1 (shoulder pitch around local X):** `75°`
* **J2 (elbow pitch around local X):** `45°`
* **J3 (wrist pitch around local X):** `15°`
* **J4 (pipette tilt around local X):** `10°`

## Visual details

* **Materials:** MeshStandardMaterial with subtle grays; different tone for each link; joints slightly darker; pipette white.
* **Lights:** 1× HemisphereLight (ambient), 1× DirectionalLight casting soft shadows.
* **Helpers:** GridHelper (size \~4 m, divisions \~40), AxesHelper (0.2 m) at world origin.
* **Shadows:** enable renderer shadows; base + links cast & receive.
* **Camera:** Perspective (fov \~60), position approximately `(x=1.6, y=1.1, z=1.8)`, lookAt world origin; **OrbitControls** allowed if already set up, otherwise position must frame the entire robot.
* **Canvas size:** full width, height 480–600px (responsive ok).

## Out-of-scope (for this task)

* Animation, controls UI, IK, physics, collisions, picking.
* Making canvas responsive. The size of the canvas is controlled in CSS.

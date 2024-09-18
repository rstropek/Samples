# Asteroids

## Introduction

This project is a simple implementation of the classic arcade game "Asteroids" using HTML5 Canvas and JavaScript. The game features a rocket launcher that can be rotated using the left and right arrow keys. Pressing the spacebar fires rockets in the direction of the launcher. The rockets move in a straight line and disappear when they move out of the canvas bounds.

[![](https://mermaid.ink/img/pako:eNqVU01v2zAM_SuCTi2aNouzpa0PA9qmwA4JCmQ7bd6BkWhbqE0FEpUmCPzf568UqdsNmC6033ukyGf6IJXVKGOpCvB-biBzUCYk6tMi4s4zOmv0A9AWvDh0XHMuOmiFpNEZyh4sMe44mgvFuxPZts4W2sHLyqpn5LMuCNeG86HQYWm3-BT4Kb23gbTv5L5P-_W7T_Tnf79iAYFUju7sVEOhXKMTQFmBHVwldDpp39fhfc7uPbT_n8pLMPSm7rcfy0Xn3mOBJRIL1b79s2gLr60tEEgYv8CU5_aFPiRXJsuH7NDAj_w7OvY6wODzX15-7X3q6HayBnyrG5BdhhzJEl0JRtfr1rqRSM7r8RMZ148aUwgFJzKhqpaGjQbGR23YOhmnUHgcSQhsv-9JyZhdwKOo39pXVWGh3kgZHyTvN81uZ8Y3tytLqckaPLiihnPmjY_H44a-ygznYX2lbDn2RufgON_ezsazaHYD0RRn11P4Mp1qtZ7c3qTR50mqrz9NIpBVNZIboJ_WlseusG162f9XTaj-AEJHFRE?type=png)](https://mermaid.live/edit#pako:eNqVU01v2zAM_SuCTi2aNouzpa0PA9qmwA4JCmQ7bd6BkWhbqE0FEpUmCPzf568UqdsNmC6033ukyGf6IJXVKGOpCvB-biBzUCYk6tMi4s4zOmv0A9AWvDh0XHMuOmiFpNEZyh4sMe44mgvFuxPZts4W2sHLyqpn5LMuCNeG86HQYWm3-BT4Kb23gbTv5L5P-_W7T_Tnf79iAYFUju7sVEOhXKMTQFmBHVwldDpp39fhfc7uPbT_n8pLMPSm7rcfy0Xn3mOBJRIL1b79s2gLr60tEEgYv8CU5_aFPiRXJsuH7NDAj_w7OvY6wODzX15-7X3q6HayBnyrG5BdhhzJEl0JRtfr1rqRSM7r8RMZ148aUwgFJzKhqpaGjQbGR23YOhmnUHgcSQhsv-9JyZhdwKOo39pXVWGh3kgZHyTvN81uZ8Y3tytLqckaPLiihnPmjY_H44a-ygznYX2lbDn2RufgON_ezsazaHYD0RRn11P4Mp1qtZ7c3qTR50mqrz9NIpBVNZIboJ_WlseusG162f9XTaj-AEJHFRE)

## Rocket Animation

The rocket animation in this project is implemented using the HTML5 Canvas API and JavaScript. Below is a detailed explanation of how the animation works, based on the provided `main.ts` and `rocket.ts` files.

### Main Animation Loop (`main.ts`)

1. **Canvas Setup**:
   - The canvas element is retrieved from the DOM using `document.getElementById('gameCanvas')`.
   - The 2D rendering context is obtained from the canvas.

2. **State Variables**:
   - `angle`: Represents the current angle of the rocket launcher.
   - `isLeftDown` and `isRightDown`: Boolean flags to track the state of the left and right arrow keys.
   - `rockets`: An array to store the rockets currently in the scene.

3. **Draw Function**:
   - The `draw` function is the main animation loop, called recursively using `requestAnimationFrame`.
   - The canvas is cleared by filling it with a black rectangle.
   - The angle of the rocket launcher is adjusted based on the state of the arrow keys.
   - The canvas context is saved and translated to the bottom center of the canvas.
   - An `AsteroidCanvas` instance is created to handle drawing operations.
   - The rocket launcher is drawn using the current angle.
   - Each rocket in the `rockets` array is drawn and moved.
   - Rockets that are out of bounds are removed from the `rockets` array.
   - The canvas context is restored to its original state.

4. **Event Listeners**:
   - `keydown` event listener updates the state variables and adds new rockets to the `rockets` array when the spacebar is pressed.
   - `keyup` event listener resets the state variables when the arrow keys are released.

### Rocket Drawing and Movement (`rocket.ts`)

1. **Rocket Type**:
   - A `Rocket` type is defined with `x`, `y`, and `angle` properties.

2. **AsteroidCanvas Extensions**:
   - `drawRocket`: A method added to the `AsteroidCanvas` prototype to draw a rocket on the canvas. It uses the rocket's position and angle to draw a yellow line representing the rocket.
   - `removeOutOfBoundsRockets`: A method added to the `AsteroidCanvas` prototype to remove rockets that have moved out of the canvas bounds.

3. **Utility Functions**:
   - `getLaunchPosition`: Calculates the initial position of a rocket based on the current angle of the rocket launcher.
   - `moveRocket`: Updates the position of a rocket based on its angle, simulating movement.

### Summary

The animation works by continuously updating the canvas in the `draw` function. The rocket launcher angle is adjusted based on user input, and rockets are drawn and moved accordingly. Rockets that move out of bounds are removed to keep the scene clean. The `AsteroidCanvas` class is extended to handle the drawing and removal of rockets, encapsulating these operations within the canvas context.
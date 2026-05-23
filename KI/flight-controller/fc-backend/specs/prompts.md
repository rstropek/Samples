## Initial Prompt

Read #01-general.md carefully. Are the requirements clear? Do you need additional information before you generate the necessary data structures and algorithms?

## Trigger

Yes, please generate the code. Note that I need the structures that represent Airplanes and Alerts to be serializable (Serde).

## Run Tests

Please run the tests with cargo test

## Lovable

I need to build a prototype for a component for an air traffic controller software. It should display a radar circle with radius 100km around an airport. A lighter circle should indicate distances of 20, 40, 60, and 80km (distance circles must be labeled in small letters).

Inside the circle, I need to display airplanes (small circles). Each airplane must be labelled with its call sign, position (lat/lng), altitude (in 100 feet). The airplanes must be selectable (one at a time). If selected, a popup must appear at the location of the airplane showing more details (airplane model, speed, direction in degrees).

Use a black background. Use green for circles and airplanes. Use red for alerts.

## UI

Take a look at the backend in the fc-backend/src folder. I need a frontend using vanillaJS for that app. Put it in fc-frontend. Use regular DOM elements for visualization (no canvas, no SVG). The project has already been set up with vite and TypeScript.

The UI should look similar to the provided screen designs.

For the UI, I do not need unit tests.

Do not change configuration files (e.g. tsconfig. vite config). I have setup the project as I like it.

## Zooming

Can you add zooming possibility? Zooming in should reduce the displayed radius by 20km. The max zoom levels should be 20km and 100km. Zooming should be done with two buttons (+ and -) at the left upper corner of the visualization.
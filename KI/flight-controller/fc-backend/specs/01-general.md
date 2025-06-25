# Introduction

## This is a Demo

This application is a demo backend for a UI displaying airplanes for air traffic controllers. The entire software is not for production use, but used to demo certain aspects of using AI for software development. Therefore, simplifying requirements is fine.

## Airplane Setup

In our demo, we simulate _Linz Airport_ (LNZ) at 48.238575/14.191473 (Austria). We simulate air traffic control in a radius of max. 100km around LNZ.

For each airplane, we know:

* Callsign (flight number, e.g. "AAL123")
* Aircraft type
* Start location (lat/lng, altitude in hundreds of feet)
* Speed (knots)
* Heading (degrees from magnetic north)

In our simplified example, the planes do not change course, change altitude, or change speed.

In our simple demo, we assume that no airplanes will be added or removed.

## Generator for Airplane Positions

We need a generator for airplane positions that receives the following input parameters:

* Generated airplanes (see parameters above)
* Elapsed time since startup (in seconds)

Assume meaningful values for speed and altitude. The callsigns should be made up of random letters and numbers (unique). Choose from a list of meaningful aircraft types.

The generator iterates over the airplanes and calculates the new position based on the speed and heading. It returns all parameters of all airplanes with the new position.

## Demo Data Generation

At startup, an algorithm generates a number of fictitious airplanes (amount is configurable using a constant in code, no need to be configurable via config file).

It is important that at least two airplanes will trigger an alert (<= 5 nautical miles appart and less than 1000 feet altitude difference) within 30 seconds after startup. The demo data generator must generate the airplanes' starting values accordingly.

## Function to Check for Alerts

We need a function that receives two airplane positions (lat/lng, altitude) and checks if they trigger a warning (rules see above). Note that you have to use the Haversine formula to calculate the distance between two points.

We need a second function that iterates over all combinations of airplanes, calls the first function for each combination, and returns all combinations that trigger an alert.

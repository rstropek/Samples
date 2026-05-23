# Introduction

## This is a Demo

This Rust application is a demo backend for a UI displaying airplanes for air traffic controllers. The entire software is not for production use, but used to demo certain aspects of using AI for software development. Therefore, simplifying requirements is fine.

## Airplane Setup

In our demo, we simulate _Linz Airport_ (LNZ) at 48.238575/14.191473 (Austria). We simulate air traffic control in a radius of max. 100km around LNZ.

For each airplane (`Airplane`), we know:

* Callsign (flight number, e.g. "AAL123")
* Aircraft type
* Start location (lat/lng, altitude in feet)
* Speed (knots)
* Heading (degrees from magnetic north)

In our simplified example, the planes do not change course, change altitude, or change speed.

In our simple demo, we assume that no airplanes will be added or removed.

## Position Calculation

We need a function `calculate_airplane_positions` that receives the original airplane data and the elapsed time since startup (in seconds) and returns a clone of the airplane data with the updated positions.

## Demo Data Generation

At startup, a function `generate_demo_airplanes` generates a number of fictitious airplanes (amount is configurable using a constant in code, no need to be configurable via config file, default value is 20). Assume meaningful values for speed and altitude. The callsigns should be made up of random letters and numbers (unique). Choose from a list of meaningful aircraft types.

The list of planes must include the following two planes (used for alert detection):

| Latitude (°)    | Longitude (°)   | Altitude (ft) | Speed (kn) | Heading (°) |
| --------------- | --------------- | ------------- | ---------- | ----------- |
| **48.288158 N** | **14.191473 E** | **30 000**    | 120        | 180 (due S) |
| **48.188992 N** | **14.191473 E** | **29 500**    | 120        | 0 (due N)   |

## Function to Check for Alerts

We need a function `check_alert_between_planes` that receives two airplane positions (lat/lng, altitude) and checks if they trigger an alert (within 5 nautical miles and less than 1000 feet altitude difference). Note that you have to use the Haversine formula to calculate the distance between two points.

Make sure to include a test of the distance calculation. Use the following test data which are approx. 1 nautical mile apart:

* Position 1: 48.250000/14.191473
* Position 2: 48.265000/14.191473

We need a second function `check_all_alerts` that iterates over all combinations of airplanes, calls the first function for each combination, and returns all combinations that trigger an alert.

## Initial Prompt

Read #01-general.md carefully. Are the requirements clear? Do you need additional information before you generate the necessary data structures and algorithms?

## Trigger

Yes, please generate the code. Note that I need the structures that represent Airplanes and Alerts to be serializable (Serde).

## Run Tests

Please run the tests with cargo test

## Test for Two Warnings

Add a test that verifies that the following plane combination leads to a warning after 15 seconds:

| Plane | Latitude (°)    | Longitude (°)   | Altitude (ft) | Speed (kn) | Heading (°) |
| ----- | --------------- | --------------- | ------------- | ---------- | ----------- |
| A     | **48.288158 N** | **14.191473 E** | **30 000**    | 120        | 180 (due S) |
| B     | **48.188992 N** | **14.191473 E** | **29 500**    | 120        | 0 (due N)   |


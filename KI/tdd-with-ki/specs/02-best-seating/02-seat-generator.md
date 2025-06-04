## Seat Generator

Write an infrastructure for generator classes (iterator) that can create seat objects for theatres. Currently, we only have one room (see [Theatre Layout](01-SeatPlan.md)). However, the code should be ready for future extensions.

### Abstraction

Create a trait `SeatGenerator` with one method:

* `generate` - returns an iterator of `Seat` objects.

### Concrete Generator

Create a generator struct implementing `SeatGenerator` that generates all seats based on the [Theatre Layout](01-SeatPlan.md).

### Acceptance Criteria

The generator for the [Theatre Layout](01-SeatPlan.md) must pass the following tests:

* Must run without throwing an exception.
* Must generate the correct total number of regular seats for each category.
* Must generate the correct total number of wheelchar seats.
* Must generate the correct total number of seats for left and right sections.

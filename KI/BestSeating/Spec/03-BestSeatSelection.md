# Best Seat Algorithm Specification

## Overview

This document specifies the algorithm for selecting the "best" seats in a theatre. The algorithm is designed to allocate a contiguous block of seats for up to 5 tickets in a single purchase. It ensures that all selected seats are side by side, not split by the main aisle, and are chosen from the highest available seating category.

## Booking Rules and Constraints

- **Ticket Limit:**  
  A single purchase can include up to 5 tickets.
- **Contiguous Seating:**  
  - All selected seats must be adjacent (side by side) within the same section.
  - Seats must not be split by the main aisle.
- **Category Selection:**  
  - The algorithm must select seats from the best available category that can accommodate the requested number of tickets.
  - Categories are prioritized in the order defined in [Theatre Layout](./01-SeatPlan.md) (from Fauteuil being the best, followed by Circle, ...).  
- **Wheelchair Requests:**  
  - If a booking includes a wheelchair requirement, seats must be allocated from the 5 reserved wheelchair spots.

## Algorithm Workflow

### Input

- Input arguments:
  - List of available seats.
  - Number of requested tickets.
  - Wheelchair requirement (true/false).
- Ensure the number of requested tickets is between 1 and 5.
- Check for a wheelchair requirement if applicable.

### Category Evaluation

- Iterate through the seating categories in order of priority:
  1. Fauteuil (F1)
  2. Circle (C1, C2)
  3. Category 3 (Rows 1–5)
  4. Category 4 (Rows 6–9)
  5. Category 5 (Rows 10–12)
  6. Category 6 (Rows 13–15)
  7. Category 7 (Rows 16–17)
  8. Category 8 (Rows 18–19)
- For each category, examine every row:
  - Check both the Parterre Left and Parterre Right sections.
  - Identify contiguous blocks of available seats that can fit the requested number of tickets.

### Best Block Selection

- From the available blocks, select the one that is:
  - In the highest-priority category.
  - Closest to the main aisle (i.e., lower seat numbers are preferred).

### Fallback

If no contiguous block of the requested size is available in any category, return an error indicating that the booking cannot be completed.

### Acceptance Critera

The best seat selector must pass the following tests:

* Returns an error if the number of requested tickets is outside the range of 1 to 5.
* Returns an error if the requested number of tickets exceeds the available seating capacity.
* Selects the best available category.
* Selects the closest block to the main aisle.

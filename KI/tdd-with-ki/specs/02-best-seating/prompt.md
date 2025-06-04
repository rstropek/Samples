#file:01-seat-plan.md  defines the seat plan for our theatre. #file:02-seat-generator.md  defines requirements for a seat generator. Create the requested trait in #file:lib.rs  and add an EMPTY implementation (todo-macro; we will add this code later). Write unit tests ONLY for the requested acceptance criteria in #file:02-seat-generator.md .

Implement #file:02-seat-generator.md in #file:lib.rs.

Move the seat generator from #file:lib.rs to a separate module named `seat_selector.rs`. Make sure to import and re-export seat generator in #file:lib.rs.

I want to implement the best seat selector in `seat_selector.rs` as specified in #file:03-seat-selection.md . Over time, there will be mulitple selectors with different algorithms. Generate a trait for the selector and add an EMPTY implementation (todo-macro; we will add this code later). Write unit tests for the requested acceptance criteria in #file:02-seat-selection.md .

Implement #file:03-seat-selection.md in #file:seat_selector.rs.

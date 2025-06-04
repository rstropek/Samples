

mod seat_generator;
mod seat_selector;

pub use seat_generator::{SeatGenerator, TheatreSeatGenerator};
pub use seat_selector::{SeatSelector, BestSeatSelector, SeatSelectionResult, SeatSelectionError};

#[derive(Debug, Clone, PartialEq)]
pub struct Seat {
    pub row: u32,
    pub seat_number: u32,
    pub category: String,
    pub section: Section,
    pub is_wheelchair: bool,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum Section {
    Left,
    Right,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_generator_runs_without_exception() {
        let generator = TheatreSeatGenerator;
        // This should not panic
        let _seats: Vec<Seat> = generator.generate().collect();
    }

    #[test]
    fn test_correct_total_number_of_regular_seats_per_category() {
        let generator = TheatreSeatGenerator;
        let seats: Vec<Seat> = generator.generate().collect();
        let regular_seats: Vec<&Seat> = seats.iter().filter(|s| !s.is_wheelchair).collect();

        // Count seats by category
        let fauteuil_count = regular_seats.iter().filter(|s| s.category == "F1").count();
        let circle_count = regular_seats.iter().filter(|s| s.category == "C1" || s.category == "C2").count();
        let category3_count = regular_seats.iter().filter(|s| s.category == "Category 3").count();
        let category4_count = regular_seats.iter().filter(|s| s.category == "Category 4").count();
        let category5_count = regular_seats.iter().filter(|s| s.category == "Category 5").count();
        let category6_count = regular_seats.iter().filter(|s| s.category == "Category 6").count();
        let category7_count = regular_seats.iter().filter(|s| s.category == "Category 7").count();
        let category8_count = regular_seats.iter().filter(|s| s.category == "Category 8").count();

        assert_eq!(fauteuil_count, 18, "Fauteuil (F1) should have 18 seats");
        assert_eq!(circle_count, 44, "Circle (C1, C2) should have 44 seats");
        assert_eq!(category3_count, 110, "Category 3 should have 110 seats");
        assert_eq!(category4_count, 88, "Category 4 should have 88 seats");
        assert_eq!(category5_count, 66, "Category 5 should have 66 seats");
        assert_eq!(category6_count, 60, "Category 6 should have 60 seats");
        assert_eq!(category7_count, 36, "Category 7 should have 36 seats");
        assert_eq!(category8_count, 30, "Category 8 should have 30 seats");
    }

    #[test]
    fn test_correct_total_number_of_wheelchair_seats() {
        let generator = TheatreSeatGenerator;
        let seats: Vec<Seat> = generator.generate().collect();
        let wheelchair_seats: Vec<&Seat> = seats.iter().filter(|s| s.is_wheelchair).collect();

        assert_eq!(wheelchair_seats.len(), 5, "Should have exactly 5 wheelchair seats");
        
        // All wheelchair seats should be on the right side
        for seat in wheelchair_seats {
            assert_eq!(seat.section, Section::Right, "All wheelchair seats should be on the right side");
        }
    }

    #[test]
    fn test_correct_total_number_of_seats_for_left_and_right_sections() {
        let generator = TheatreSeatGenerator;
        let seats: Vec<Seat> = generator.generate().collect();

        let left_seats: Vec<&Seat> = seats.iter().filter(|s| s.section == Section::Left).collect();
        let right_seats: Vec<&Seat> = seats.iter().filter(|s| s.section == Section::Right).collect();

        // Calculate expected counts based on the specification
        // Left side: only regular seats
        let expected_left_regular = 9 + 22 + 55 + 44 + 33 + 30 + 18 + 18; // 229
        
        // Right side: regular seats + 5 wheelchair seats
        let expected_right_regular = 9 + 22 + 55 + 44 + 33 + 30 + 18 + 12; // 223 (note: Category 8 right has only 6 seats)
        let expected_right_wheelchair = 5;

        let left_regular_count = left_seats.iter().filter(|s| !s.is_wheelchair).count();
        let right_regular_count = right_seats.iter().filter(|s| !s.is_wheelchair).count();
        let right_wheelchair_count = right_seats.iter().filter(|s| s.is_wheelchair).count();

        assert_eq!(left_regular_count, expected_left_regular, "Left section should have {} regular seats", expected_left_regular);
        assert_eq!(right_regular_count, expected_right_regular, "Right section should have {} regular seats", expected_right_regular);
        assert_eq!(right_wheelchair_count, expected_right_wheelchair, "Right section should have {} wheelchair seats", expected_right_wheelchair);

        // Total verification
        let total_seats = seats.len();
        assert_eq!(total_seats, 452 + 5, "Total seats should be 457 (452 regular + 5 wheelchair)");
    }
}

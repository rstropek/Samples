use crate::{Seat, Section};

/// Result type for seat selection operations
pub type SeatSelectionResult = Result<Vec<Seat>, SeatSelectionError>;

/// Errors that can occur during seat selection
#[derive(Debug, Clone, PartialEq)]
pub enum SeatSelectionError {
    InvalidTicketCount(String),
    InsufficientSeats(String),
}

/// Trait for different seat selection algorithms
pub trait SeatSelector {
    /// Selects the best available seats based on the algorithm
    /// 
    /// # Arguments
    /// * `available_seats` - List of all available seats
    /// * `ticket_count` - Number of tickets requested (1-5)
    /// * `wheelchair_required` - Whether wheelchair-accessible seats are required
    /// 
    /// # Returns
    /// * `Ok(Vec<Seat>)` - Vector of selected seats
    /// * `Err(SeatSelectionError)` - Error if selection fails
    fn select_seats(
        &self,
        available_seats: &[Seat],
        ticket_count: u32,
        wheelchair_required: bool,
    ) -> SeatSelectionResult;
}

/// Best seat selector implementation following the priority-based algorithm
pub struct BestSeatSelector;

impl SeatSelector for BestSeatSelector {
    fn select_seats(
        &self,
        available_seats: &[Seat],
        ticket_count: u32,
        wheelchair_required: bool,
    ) -> SeatSelectionResult {
        // Validate ticket count
        if ticket_count == 0 || ticket_count > 5 {
            return Err(SeatSelectionError::InvalidTicketCount(
                format!("Ticket count must be between 1 and 5, got {}", ticket_count)
            ));
        }

        // Filter seats based on wheelchair requirement
        let filtered_seats: Vec<&Seat> = available_seats
            .iter()
            .filter(|seat| seat.is_wheelchair == wheelchair_required)
            .collect();

        // Check if we have enough seats
        if filtered_seats.len() < ticket_count as usize {
            return Err(SeatSelectionError::InsufficientSeats(
                format!("Not enough seats available. Requested: {}, Available: {}", 
                    ticket_count, filtered_seats.len())
            ));
        }

        // Define category priority order
        let category_priority = if wheelchair_required {
            vec!["Wheelchair"]
        } else {
            vec!["F1", "C1", "C2", "Category 3", "Category 4", "Category 5", 
                 "Category 6", "Category 7", "Category 8"]
        };

        // Try each category in priority order
        for category in category_priority {
            if let Some(selected_seats) = self.find_best_block_in_category(
                &filtered_seats, 
                category, 
                ticket_count
            ) {
                return Ok(selected_seats);
            }
        }

        // If no contiguous block found
        Err(SeatSelectionError::InsufficientSeats(
            format!("Could not find {} contiguous seats", ticket_count)
        ))
    }
}

impl BestSeatSelector {
    /// Find the best contiguous block of seats in a specific category
    fn find_best_block_in_category(
        &self,
        seats: &[&Seat],
        category: &str,
        ticket_count: u32,
    ) -> Option<Vec<Seat>> {
        // Filter seats for this category
        let category_seats: Vec<&Seat> = seats
            .iter()
            .filter(|seat| seat.category == category)
            .copied()
            .collect();

        if category_seats.is_empty() {
            return None;
        }

        // Group by row and section
        use std::collections::HashMap;
        let mut groups: HashMap<(u32, Section), Vec<&Seat>> = HashMap::new();
        
        for seat in category_seats {
            let key = (seat.row, seat.section.clone());
            groups.entry(key).or_insert_with(Vec::new).push(seat);
        }

        // Sort each group by seat number and find contiguous blocks
        let mut best_block: Option<Vec<Seat>> = None;
        let mut best_start_seat_number = u32::MAX;

        for ((_row, _section), mut group_seats) in groups {
            // Sort by seat number
            group_seats.sort_by_key(|seat| seat.seat_number);
            
            // Find contiguous blocks in this group
            if let Some(block) = self.find_contiguous_block(&group_seats, ticket_count) {
                let start_seat_number = block[0].seat_number;
                
                // Prefer blocks that start with lower seat numbers (closer to aisle)
                if start_seat_number < best_start_seat_number {
                    best_start_seat_number = start_seat_number;
                    best_block = Some(block);
                }
            }
        }

        best_block
    }

    /// Find a contiguous block of the requested size in a sorted group of seats
    fn find_contiguous_block(
        &self,
        sorted_seats: &[&Seat],
        ticket_count: u32,
    ) -> Option<Vec<Seat>> {
        if sorted_seats.len() < ticket_count as usize {
            return None;
        }

        // Look for contiguous blocks
        for i in 0..=(sorted_seats.len() - ticket_count as usize) {
            let mut is_contiguous = true;
            
            // Check if the next ticket_count seats are contiguous
            for j in 1..ticket_count as usize {
                if sorted_seats[i + j].seat_number != sorted_seats[i + j - 1].seat_number + 1 {
                    is_contiguous = false;
                    break;
                }
            }
            
            if is_contiguous {
                // Found a contiguous block - return it
                let block: Vec<Seat> = sorted_seats[i..i + ticket_count as usize]
                    .iter()
                    .map(|&seat| seat.clone())
                    .collect();
                return Some(block);
            }
        }

        None
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::{Section, SeatGenerator, TheatreSeatGenerator};

    fn create_test_seats() -> Vec<Seat> {
        let generator = TheatreSeatGenerator;
        generator.generate().collect()
    }

    fn mark_seats_as_unavailable(seats: &mut Vec<Seat>, unavailable_positions: &[(u32, u32, Section)]) {
        for (row, seat_num, section) in unavailable_positions {
            if let Some(pos) = seats.iter().position(|s| 
                s.row == *row && s.seat_number == *seat_num && s.section == *section && !s.is_wheelchair
            ) {
                seats.remove(pos);
            }
        }
    }

    #[test]
    fn test_returns_error_for_invalid_ticket_count_zero() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        let result = selector.select_seats(&seats, 0, false);
        assert!(result.is_err());
        match result {
            Err(SeatSelectionError::InvalidTicketCount(_)) => {},
            _ => panic!("Expected InvalidTicketCount error for 0 tickets"),
        }
    }

    #[test]
    fn test_returns_error_for_invalid_ticket_count_too_high() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        let result = selector.select_seats(&seats, 6, false);
        assert!(result.is_err());
        match result {
            Err(SeatSelectionError::InvalidTicketCount(_)) => {},
            _ => panic!("Expected InvalidTicketCount error for 6 tickets"),
        }
    }

    #[test]
    fn test_returns_error_when_insufficient_seats() {
        let selector = BestSeatSelector;
        let mut seats = create_test_seats();
        
        // Remove most seats to simulate full theatre
        seats.retain(|s| s.category == "F1" && s.seat_number <= 2);
        
        let result = selector.select_seats(&seats, 5, false);
        assert!(result.is_err());
        match result {
            Err(SeatSelectionError::InsufficientSeats(_)) => {},
            _ => panic!("Expected InsufficientSeats error when not enough seats available"),
        }
    }

    #[test]
    fn test_selects_best_available_category_fauteuil() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        let result = selector.select_seats(&seats, 2, false);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        assert_eq!(selected_seats.len(), 2);
        
        // Should select from Fauteuil (F1) as it's the best category
        for seat in &selected_seats {
            assert_eq!(seat.category, "F1");
        }
        
        // Should be contiguous
        let seat_numbers: Vec<u32> = selected_seats.iter().map(|s| s.seat_number).collect();
        assert_eq!(seat_numbers, vec![1, 2]);
    }

    #[test]
    fn test_selects_best_available_category_circle_when_fauteuil_unavailable() {
        let selector = BestSeatSelector;
        let mut seats = create_test_seats();
        
        // Remove all Fauteuil seats
        seats.retain(|s| s.category != "F1");
        
        let result = selector.select_seats(&seats, 3, false);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        assert_eq!(selected_seats.len(), 3);
        
        // Should select from Circle (C1 or C2) as it's the next best category
        for seat in &selected_seats {
            assert!(seat.category == "C1" || seat.category == "C2");
        }
        
        // Should be contiguous
        let seat_numbers: Vec<u32> = selected_seats.iter().map(|s| s.seat_number).collect();
        assert_eq!(seat_numbers, vec![1, 2, 3]);
    }

    #[test]
    fn test_selects_seats_closest_to_main_aisle() {
        let selector = BestSeatSelector;
        let mut seats = create_test_seats();
        
        // Remove seats 1-3 from Fauteuil left to force selection from seat 4 onwards
        mark_seats_as_unavailable(&mut seats, &[
            (1, 1, Section::Left),
            (1, 2, Section::Left),
            (1, 3, Section::Left),
        ]);
        
        let result = selector.select_seats(&seats, 2, false);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        assert_eq!(selected_seats.len(), 2);
        
        // Should select from Fauteuil right side (seats closest to aisle: 1, 2)
        // since left side seats 1-3 are unavailable
        for seat in &selected_seats {
            assert_eq!(seat.category, "F1");
            assert_eq!(seat.section, Section::Right);
        }
        
        let seat_numbers: Vec<u32> = selected_seats.iter().map(|s| s.seat_number).collect();
        assert_eq!(seat_numbers, vec![1, 2]);
    }

    #[test]
    fn test_wheelchair_seats_selection() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        let result = selector.select_seats(&seats, 2, true);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        assert_eq!(selected_seats.len(), 2);
        
        // All selected seats should be wheelchair accessible
        for seat in &selected_seats {
            assert!(seat.is_wheelchair);
            assert_eq!(seat.section, Section::Right);
        }
    }

    #[test]
    fn test_no_mixed_regular_and_wheelchair_seats() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        // Test regular seats request
        let result = selector.select_seats(&seats, 3, false);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        for seat in &selected_seats {
            assert!(!seat.is_wheelchair, "Regular booking should not include wheelchair seats");
        }
        
        // Test wheelchair seats request
        let result = selector.select_seats(&seats, 2, true);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        for seat in &selected_seats {
            assert!(seat.is_wheelchair, "Wheelchair booking should only include wheelchair seats");
        }
    }

    #[test]
    fn test_seats_are_contiguous_and_not_split_by_aisle() {
        let selector = BestSeatSelector;
        let seats = create_test_seats();
        
        let result = selector.select_seats(&seats, 4, false);
        assert!(result.is_ok());
        
        let selected_seats = result.unwrap();
        assert_eq!(selected_seats.len(), 4);
        
        // All seats should be in the same section (not split by aisle)
        let first_section = &selected_seats[0].section;
        for seat in &selected_seats {
            assert_eq!(&seat.section, first_section, "Seats should not be split by main aisle");
        }
        
        // All seats should be in the same row
        let first_row = selected_seats[0].row;
        for seat in &selected_seats {
            assert_eq!(seat.row, first_row, "Seats should be in the same row");
        }
        
        // Seat numbers should be consecutive
        let mut seat_numbers: Vec<u32> = selected_seats.iter().map(|s| s.seat_number).collect();
        seat_numbers.sort();
        for i in 1..seat_numbers.len() {
            assert_eq!(
                seat_numbers[i], 
                seat_numbers[i-1] + 1, 
                "Seat numbers should be consecutive"
            );
        }
    }
}

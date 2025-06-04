use crate::{Seat, Section};

pub trait SeatGenerator {
    fn generate(&self) -> Box<dyn Iterator<Item = Seat>>;
}

pub struct TheatreSeatGenerator;

impl SeatGenerator for TheatreSeatGenerator {
    fn generate(&self) -> Box<dyn Iterator<Item = Seat>> {
        let mut seats = Vec::new();
        let mut row_number = 1;

        // Fauteuil (F1) - 1 row, 9 seats each side
        for seat_num in 1..=9 {
            seats.push(Seat {
                row: row_number,
                seat_number: seat_num,
                category: "F1".to_string(),
                section: Section::Left,
                is_wheelchair: false,
            });
            seats.push(Seat {
                row: row_number,
                seat_number: seat_num,
                category: "F1".to_string(),
                section: Section::Right,
                is_wheelchair: false,
            });
        }
        row_number += 1;

        // Circle (C1, C2) - 2 rows, 11 seats each side
        for row in 0..2 {
            let category = if row == 0 { "C1" } else { "C2" };
            for seat_num in 1..=11 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: category.to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: category.to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 3 - 5 rows, 11 seats each side
        for _ in 0..5 {
            for seat_num in 1..=11 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 3".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 3".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 4 - 4 rows, 11 seats each side
        for _ in 0..4 {
            for seat_num in 1..=11 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 4".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 4".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 5 - 3 rows, 11 seats each side
        for _ in 0..3 {
            for seat_num in 1..=11 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 5".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 5".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 6 - 3 rows, 10 seats each side
        for _ in 0..3 {
            for seat_num in 1..=10 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 6".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 6".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 7 - 2 rows, 9 seats each side
        for _ in 0..2 {
            for seat_num in 1..=9 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 7".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 7".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Category 8 - 2 rows, 9 seats left side, 6 seats right side
        for _ in 0..2 {
            // Left side: 9 seats
            for seat_num in 1..=9 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 8".to_string(),
                    section: Section::Left,
                    is_wheelchair: false,
                });
            }
            // Right side: 6 seats
            for seat_num in 1..=6 {
                seats.push(Seat {
                    row: row_number,
                    seat_number: seat_num,
                    category: "Category 8".to_string(),
                    section: Section::Right,
                    is_wheelchair: false,
                });
            }
            row_number += 1;
        }

        // Wheelchair seats - 5 seats, all on right side
        // We'll add them to a separate "wheelchair row" for simplicity
        for seat_num in 1..=5 {
            seats.push(Seat {
                row: row_number,
                seat_number: seat_num,
                category: "Wheelchair".to_string(),
                section: Section::Right,
                is_wheelchair: true,
            });
        }

        Box::new(seats.into_iter())
    }
}

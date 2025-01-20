pub struct ScrewShelf {
    no_of_columns: u16,
    no_of_rows: u16,
    width_of_column: f32,
    height_of_row: f32,
}

impl ScrewShelf {
    pub fn new(no_of_columns: u16, no_of_rows: u16, width_of_column: f32, height_of_row: f32) -> Self {
        Self { no_of_columns, no_of_rows, width_of_column, height_of_row }
    }

    pub fn get_shelf_position(&self, ix: (u16, u16)) -> Result<(f32, f32), &'static str> {
        if ix.0 >= self.no_of_columns || ix.1 >= self.no_of_rows {
            return Err("Invalid shelf position");
        }

        let x = (ix.0 as f32) * self.width_of_column;
        let y = (ix.1 as f32) * self.height_of_row;
        Ok((x, y))
    }

    #[allow(dead_code)]
    pub fn get_travel_distance(&self, from: (u16, u16), to: (u16, u16)) -> Result<(f32, f32, f32), &'static str> {
        let (x1, y1) = self.get_shelf_position(from)?;
        let (x2, y2) = self.get_shelf_position(to)?;
        let dx = x2 - x1;
        let dy = y2 - y1;
        let distance = ((x2 - x1).powi(2) + (y2 - y1).powi(2)).sqrt();
        Ok((dx, dy, distance))
    }

    pub fn get_shelf_ix(&self, pos: (f32, f32)) -> Result<(u16, u16), &'static str> {
        // Round to 2 decimal places to handle floating point imprecision
        let x = (pos.0 * 100.0).round() / 100.0;
        let y = (pos.1 * 100.0).round() / 100.0;

        // Calculate column index by subtracting half column width and dividing by column width
        let col = (x / self.width_of_column).round() as i32;
        let row = (y / self.height_of_row).round() as i32;

        // Validate the calculated indices
        if col < 0 || row < 0 || col >= self.no_of_columns as i32 || row >= self.no_of_rows as i32 {
            return Err("Position does not correspond to a valid shelf location");
        }

        // Verify that this position actually corresponds to a shelf position
        let (shelf_x, shelf_y) = self.get_shelf_position((col as u16, row as u16))?;
        let rounded_shelf_x = (shelf_x * 100.0).round() / 100.0;
        let rounded_shelf_y = (shelf_y * 100.0).round() / 100.0;

        if rounded_shelf_x != x || rounded_shelf_y != y {
            return Err("Position does not correspond to a valid shelf location, perform a homing operation");
        }

        Ok((col as u16, row as u16))
    }
}

#[cfg(test)]
mod tests {
    use rstest::rstest;
    use assert_approx_eq::assert_approx_eq;

    use super::*;

    #[rstest]
    #[case((0, 0), (0.0, 0.0))]
    #[case((1, 1), (1.0, 1.0))]
    #[case((2, 2), (2.0, 2.0))]
    fn test_get_shelf_position(#[case] ix: (u16, u16), #[case] expected: (f32, f32)) {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let position = warehouse.get_shelf_position(ix);
        assert_eq!(position, Ok(expected));
    }

    #[test]
    fn test_get_shelf_position_invalid_index() {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let position = warehouse.get_shelf_position((10, 10));
        assert!(position.is_err());
    }

    #[rstest]
    #[case((0, 0), (1, 0), (1.0, 0.0, 1.0))]
    #[case((0, 0), (0, 1), (0.0, 1.0, 1.0))]
    #[case((0, 0), (1, 1), (1.0, 1.0, 2.0_f32.sqrt()))]
    #[case((0, 1), (0, 0), (0.0, -1.0, 1.0))]
    fn test_get_travel_distance(
        #[case] from: (u16, u16),
        #[case] to: (u16, u16),
        #[case] expected: (f32, f32, f32),
    ) {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let distance = warehouse.get_travel_distance(from, to);
        let actual = distance.unwrap();
        assert_approx_eq!(actual.0, expected.0, 0.001);
        assert_approx_eq!(actual.1, expected.1, 0.001);
        assert_approx_eq!(actual.2, expected.2, 0.001);
    }

    #[test]
    fn test_get_travel_distance_invalid_position() {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let distance = warehouse.get_travel_distance((0, 0), (10, 10));
        assert!(distance.is_err());
    }

    #[rstest]
    #[case((0.0, 0.0), (0, 0))]
    #[case((1.0, 1.0), (1, 1))]
    #[case((2.0, 2.0), (2, 2))]
    fn test_get_shelf_ix(#[case] pos: (f32, f32), #[case] expected: (u16, u16)) {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let ix = warehouse.get_shelf_ix(pos);
        assert_eq!(ix, Ok(expected));
    }

    #[rstest]
    #[case((0.75, 0.0))]  // Invalid x position
    #[case((0.5, 0.5))]   // Invalid y position
    #[case((10.5, 0.0))]  // Out of bounds x
    #[case((0.5, 10.0))]  // Out of bounds y
    fn test_get_shelf_ix_invalid_position(#[case] pos: (f32, f32)) {
        let warehouse = ScrewShelf::new(10, 10, 1.0, 1.0);
        let ix = warehouse.get_shelf_ix(pos);
        assert!(ix.is_err());
    }
}
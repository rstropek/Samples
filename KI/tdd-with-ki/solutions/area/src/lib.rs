/// Represents a survey measurement point with distance and angle
#[derive(Debug, Clone, PartialEq)]
pub struct SurveyPoint {
    /// Distance from the previous point in meters
    pub distance: f64,
    /// Angle in degrees relative to magnetic north
    pub angle: f64,
}

/// Represents a coordinate point in x/y space
#[derive(Debug, Clone, PartialEq)]
pub struct Coordinate {
    pub x: f64,
    pub y: f64,
}

impl SurveyPoint {
    /// Creates a new survey point
    pub fn new(distance: f64, angle: f64) -> Self {
        Self { distance, angle }
    }
}

impl Coordinate {
    /// Creates a new coordinate point
    pub fn new(x: f64, y: f64) -> Self {
        Self { x, y }
    }
}

/// Converts survey points to x/y coordinates relative to the starting point
/// 
/// The first coordinate will always be (0, 0) representing the starting point.
/// Each subsequent coordinate is calculated based on the distance and angle
/// from the previous point.
/// 
/// # Arguments
/// * `survey_points` - Vector of survey measurements
/// 
/// # Returns
/// Vector of coordinates starting from (0, 0)
pub fn survey_points_to_coordinates(survey_points: &[SurveyPoint]) -> Vec<Coordinate> {
    let mut coordinates = vec![Coordinate::new(0.0, 0.0)];
    
    for point in survey_points {
        let last_coord = coordinates.last().unwrap();
        
        // Convert angle from degrees to radians
        let angle_rad = point.angle.to_radians();
        
        // Calculate new position using trigonometry
        // North is positive Y, East is positive X
        let x = last_coord.x + point.distance * angle_rad.sin();
        let y = last_coord.y + point.distance * angle_rad.cos();
        
        coordinates.push(Coordinate::new(x, y));
    }
    
    coordinates
}

/// Calculates the area of a plot based on coordinates using the Shoelace formula
/// 
/// This function assumes the coordinates form a closed polygon.
/// If not already closed, it will automatically close the polygon by connecting
/// the last point back to the first point.
/// 
/// # Arguments
/// * `coordinates` - Vector of coordinate points forming the polygon
/// 
/// # Returns
/// The area of the polygon in square units
pub fn calculate_area(coordinates: &[Coordinate]) -> f64 {
    if coordinates.len() < 3 {
        return 0.0;
    }
    
    let mut area = 0.0;
    let n = coordinates.len();
    
    // Apply the Shoelace formula
    for i in 0..n {
        let j = (i + 1) % n;
        area += coordinates[i].x * coordinates[j].y;
        area -= coordinates[j].x * coordinates[i].y;
    }
    
    // Return absolute value and divide by 2
    area.abs() / 2.0
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_survey_point_creation() {
        let point = SurveyPoint::new(10.0, 45.0);
        assert_eq!(point.distance, 10.0);
        assert_eq!(point.angle, 45.0);
    }

    #[test]
    fn test_coordinate_creation() {
        let coord = Coordinate::new(5.0, 10.0);
        assert_eq!(coord.x, 5.0);
        assert_eq!(coord.y, 10.0);
    }

    #[test]
    fn test_simple_square_coordinates() {
        let survey_points = vec![
            SurveyPoint::new(10.0, 0.0),   // North 10 units
            SurveyPoint::new(10.0, 90.0), // East 10 units
            SurveyPoint::new(10.0, 180.0), // South 10 units
            SurveyPoint::new(10.0, 270.0), // West 10 units
        ];
        
        let coordinates = survey_points_to_coordinates(&survey_points);
        
        assert_eq!(coordinates.len(), 5);
        assert_eq!(coordinates[0], Coordinate::new(0.0, 0.0));
        
        // Check that we get approximately the right coordinates for a square
        assert!((coordinates[1].x - 0.0).abs() < 0.001);
        assert!((coordinates[1].y - 10.0).abs() < 0.001);
    }

    #[test]
    fn test_area_calculation_square() {
        let coordinates = vec![
            Coordinate::new(0.0, 0.0),
            Coordinate::new(0.0, 10.0),
            Coordinate::new(10.0, 10.0),
            Coordinate::new(10.0, 0.0),
        ];
        
        let area = calculate_area(&coordinates);
        assert_eq!(area, 100.0);
    }

    #[test]
    fn test_area_calculation_triangle() {
        let coordinates = vec![
            Coordinate::new(0.0, 0.0),
            Coordinate::new(10.0, 0.0),
            Coordinate::new(5.0, 10.0),
        ];
        
        let area = calculate_area(&coordinates);
        assert_eq!(area, 50.0);
    }

    #[test]
    fn test_empty_coordinates() {
        let coordinates = vec![];
        let area = calculate_area(&coordinates);
        assert_eq!(area, 0.0);
    }
}
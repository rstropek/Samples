use std::f64::consts::PI;
use rand::Rng;

// Constants
const NUM_AIRPLANES: usize = 8;
const LINZ_AIRPORT_LAT: f64 = 48.238575;
const LINZ_AIRPORT_LNG: f64 = 14.191473;
const MAX_RADIUS_KM: f64 = 100.0;
const ALERT_DISTANCE_NM: f64 = 5.0;
const ALERT_ALTITUDE_DIFF_FT: f64 = 1000.0;
const NM_TO_KM: f64 = 1.852;
const KNOTS_TO_KM_PER_HOUR: f64 = 1.852;

#[derive(Debug, Clone)]
pub struct Position {
    pub lat: f64,
    pub lng: f64,
}

#[derive(Debug, Clone)]
pub struct Airplane {
    pub callsign: String,
    pub aircraft_type: String,
    pub position: Position,
    pub altitude: f64, // in hundreds of feet
    pub speed: f64,    // in knots
    pub heading: f64,  // degrees from magnetic north
}

#[derive(Debug, Clone)]
pub struct Alert {
    pub airplane1: String,
    pub airplane2: String,
    pub distance_nm: f64,
    pub altitude_diff_ft: f64,
}

impl Airplane {
    pub fn new(
        callsign: String,
        aircraft_type: String,
        position: Position,
        altitude: f64,
        speed: f64,
        heading: f64,
    ) -> Self {
        Self {
            callsign,
            aircraft_type,
            position,
            altitude,
            speed,
            heading,
        }
    }

    /// Update airplane position based on elapsed time in seconds
    pub fn update_position(&mut self, elapsed_seconds: f64) {
        let distance_km = (self.speed * KNOTS_TO_KM_PER_HOUR * elapsed_seconds) / 3600.0;
        
        // Convert heading to radians
        let heading_rad = (self.heading * PI) / 180.0;
        
        // Calculate new position using basic trigonometry
        // Note: This is a simplified calculation that works for small distances
        let lat_change = (distance_km * heading_rad.cos()) / 111.32; // ~111.32 km per degree of latitude
        let lng_change = (distance_km * heading_rad.sin()) / (111.32 * self.position.lat.to_radians().cos());
        
        self.position.lat += lat_change;
        self.position.lng += lng_change;
    }
}

/// Calculate distance between two positions using Haversine formula
pub fn haversine_distance(pos1: &Position, pos2: &Position) -> f64 {
    let lat1_rad = pos1.lat.to_radians();
    let lat2_rad = pos2.lat.to_radians();
    let delta_lat = (pos2.lat - pos1.lat).to_radians();
    let delta_lng = (pos2.lng - pos1.lng).to_radians();

    let a = (delta_lat / 2.0).sin().powi(2)
        + lat1_rad.cos() * lat2_rad.cos() * (delta_lng / 2.0).sin().powi(2);
    let c = 2.0 * a.sqrt().atan2((1.0 - a).sqrt());

    let distance_km = 6371.0 * c; // Earth's radius in km
    distance_km / NM_TO_KM // Convert to nautical miles
}

/// Check if two airplanes trigger an alert
pub fn check_alert(airplane1: &Airplane, airplane2: &Airplane) -> Option<Alert> {
    let distance_nm = haversine_distance(&airplane1.position, &airplane2.position);
    let altitude_diff_ft = (airplane1.altitude - airplane2.altitude).abs() * 100.0; // Convert to feet

    if distance_nm <= ALERT_DISTANCE_NM && altitude_diff_ft < ALERT_ALTITUDE_DIFF_FT {
        Some(Alert {
            airplane1: airplane1.callsign.clone(),
            airplane2: airplane2.callsign.clone(),
            distance_nm,
            altitude_diff_ft,
        })
    } else {
        None
    }
}

/// Check all airplane combinations for alerts
pub fn check_all_alerts(airplanes: &[Airplane]) -> Vec<Alert> {
    let mut alerts = Vec::new();

    for i in 0..airplanes.len() {
        for j in (i + 1)..airplanes.len() {
            if let Some(alert) = check_alert(&airplanes[i], &airplanes[j]) {
                alerts.push(alert);
            }
        }
    }

    alerts
}

/// Generate a random callsign (3 letters + 3-4 digits)
fn generate_callsign() -> String {
    let mut rng = rand::thread_rng();
    let letters: String = (0..3)
        .map(|_| {
            let c = rng.gen_range(b'A'..=b'Z') as char;
            c
        })
        .collect();
    let numbers: u16 = rng.gen_range(100..=9999);
    format!("{}{}", letters, numbers)
}

/// Generate a random position within radius of Linz Airport
fn generate_position_near_linz(max_radius_km: f64) -> Position {
    let mut rng = rand::thread_rng();
    
    // Generate random distance and angle
    let distance_km = rng.gen_range(5.0..max_radius_km);
    let angle_rad = rng.gen_range(0.0..2.0 * PI);
    
    // Convert to lat/lng offset
    let lat_offset = (distance_km * angle_rad.cos()) / 111.32;
    let lng_offset = (distance_km * angle_rad.sin()) / (111.32 * LINZ_AIRPORT_LAT.to_radians().cos());
    
    Position {
        lat: LINZ_AIRPORT_LAT + lat_offset,
        lng: LINZ_AIRPORT_LNG + lng_offset,
    }
}

/// Generate demo airplane data
pub fn generate_demo_airplanes() -> Vec<Airplane> {
    let mut rng = rand::thread_rng();
    let mut airplanes = Vec::new();
    
    let aircraft_types = vec![
        "A320", "A321", "A330", "A350", "A380",
        "B737", "B747", "B767", "B777", "B787",
        "E190", "CRJ900", "ATR72"
    ];

    // Generate regular airplanes
    for _ in 0..(NUM_AIRPLANES - 2) {
        let callsign = generate_callsign();
        let aircraft_type = aircraft_types[rng.gen_range(0..aircraft_types.len())].to_string();
        let position = generate_position_near_linz(MAX_RADIUS_KM);
        let altitude = rng.gen_range(50..400) as f64; // 5000-40000 feet in hundreds
        let speed = rng.gen_range(150..500) as f64; // knots
        let heading = rng.gen_range(0..360) as f64; // degrees
        
        airplanes.push(Airplane::new(
            callsign,
            aircraft_type,
            position,
            altitude,
            speed,
            heading,
        ));
    }

    // Generate two airplanes that will trigger an alert within 30 seconds
    // Place them close together with converging paths
    let base_position = generate_position_near_linz(50.0);
    
    // First airplane
    let callsign1 = generate_callsign();
    let aircraft_type1 = aircraft_types[rng.gen_range(0..aircraft_types.len())].to_string();
    let altitude1 = 250.0; // 25000 feet
    let speed1 = 300.0; // knots
    let heading1 = 90.0; // heading east
    
    airplanes.push(Airplane::new(
        callsign1,
        aircraft_type1,
        Position {
            lat: base_position.lat,
            lng: base_position.lng - 0.05, // Start 5.5km west
        },
        altitude1,
        speed1,
        heading1,
    ));

    // Second airplane - will converge with first
    let callsign2 = generate_callsign();
    let aircraft_type2 = aircraft_types[rng.gen_range(0..aircraft_types.len())].to_string();
    let altitude2 = 260.0; // 26000 feet (600 feet difference)
    let speed2 = 280.0; // knots
    let heading2 = 270.0; // heading west
    
    airplanes.push(Airplane::new(
        callsign2,
        aircraft_type2,
        Position {
            lat: base_position.lat,
            lng: base_position.lng + 0.05, // Start 5.5km east
        },
        altitude2,
        speed2,
        heading2,
    ));

    airplanes
}

/// Update all airplane positions based on elapsed time
pub fn update_airplane_positions(airplanes: &mut [Airplane], elapsed_seconds: f64) {
    for airplane in airplanes.iter_mut() {
        airplane.update_position(elapsed_seconds);
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_haversine_distance() {
        let pos1 = Position { lat: 48.238575, lng: 14.191473 }; // Linz Airport
        let pos2 = Position { lat: 48.238575, lng: 14.291473 }; // ~7.4 km east
        
        let distance = haversine_distance(&pos1, &pos2);
        assert!((distance - 4.0).abs() < 1.0); // Should be around 4 NM
    }

    #[test]
    fn test_alert_detection() {
        let airplane1 = Airplane::new(
            "TEST1".to_string(),
            "A320".to_string(),
            Position { lat: 48.238575, lng: 14.191473 },
            250.0,
            300.0,
            90.0,
        );
        
        let airplane2 = Airplane::new(
            "TEST2".to_string(),
            "B737".to_string(),
            Position { lat: 48.238575, lng: 14.201473 }, // Close position
            255.0, // 500 feet difference
            280.0,
            270.0,
        );

        let alert = check_alert(&airplane1, &airplane2);
        assert!(alert.is_some());
    }

    #[test]
    fn test_airplane_position_update() {
        let mut airplane = Airplane::new(
            "TEST1".to_string(),
            "A320".to_string(),
            Position { lat: 48.238575, lng: 14.191473 },
            250.0,
            300.0, // 300 knots
            90.0,  // heading east
        );

        let initial_lng = airplane.position.lng;
        airplane.update_position(3600.0); // 1 hour
        
        // Should have moved east (longitude should increase)
        assert!(airplane.position.lng > initial_lng);
    }

    #[test]
    fn test_demo_data_generation() {
        let airplanes = generate_demo_airplanes();
        assert_eq!(airplanes.len(), NUM_AIRPLANES);
        
        // Check that all callsigns are unique
        let mut callsigns: Vec<_> = airplanes.iter().map(|a| &a.callsign).collect();
        callsigns.sort();
        callsigns.dedup();
        assert_eq!(callsigns.len(), NUM_AIRPLANES);
    }

    #[test]
    fn test_converging_planes_alert_after_15_seconds() {
        // Create Plane A: 48.288158°N, 14.191473°E, 30,000 ft, 120 kn, heading 180° (due S)
        let mut airplane_a = Airplane::new(
            "TEST_A".to_string(),
            "A320".to_string(),
            Position { lat: 48.288158, lng: 14.191473 },
            300.0, // 30,000 feet in hundreds
            120.0, // 120 knots
            180.0, // due south
        );

        // Create Plane B: 48.188992°N, 14.191473°E, 29,500 ft, 120 kn, heading 0° (due N)
        let mut airplane_b = Airplane::new(
            "TEST_B".to_string(),
            "B737".to_string(),
            Position { lat: 48.188992, lng: 14.191473 },
            295.0, // 29,500 feet in hundreds
            120.0, // 120 knots
            0.0,   // due north
        );

        // Verify no alert initially (they should be > 5 NM apart)
        let initial_alert = check_alert(&airplane_a, &airplane_b);
        assert!(initial_alert.is_none(), "Should not have alert initially");

        // Update positions after 15 seconds
        airplane_a.update_position(15.0);
        airplane_b.update_position(15.0);

        // Verify alert is triggered after 15 seconds
        let final_alert = check_alert(&airplane_a, &airplane_b);
        assert!(final_alert.is_some(), "Should have alert after 15 seconds");

        if let Some(alert) = final_alert {
            // Verify the alert details make sense
            assert!(alert.distance_nm <= ALERT_DISTANCE_NM, "Distance should be <= 5 NM");
            assert!(alert.altitude_diff_ft < ALERT_ALTITUDE_DIFF_FT, "Altitude difference should be < 1000 ft");
            
            // Check that distance is approximately what we expect (~4.96 NM)
            assert!(alert.distance_nm > 4.0 && alert.distance_nm < 5.0, 
                    "Distance should be around 4.96 NM, got: {}", alert.distance_nm);
            
            // Check altitude difference (should be 500 feet)
            assert!((alert.altitude_diff_ft - 500.0).abs() < 1.0, 
                    "Altitude difference should be 500 ft, got: {}", alert.altitude_diff_ft);
        }
    }
}

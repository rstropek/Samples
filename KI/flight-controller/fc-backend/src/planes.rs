use serde::{Deserialize, Serialize};
use rand::Rng;

/// Number of demo airplanes to generate (configurable constant)
const NUM_DEMO_PLANES: usize = 20;

/// Linz Airport coordinates
const LNZ_LAT: f64 = 48.238575;
const LNZ_LNG: f64 = 14.191473;

/// Alert thresholds
const ALERT_DISTANCE_NM: f64 = 5.0;
const ALERT_ALTITUDE_DIFF_FT: f64 = 1000.0;

/// Conversion constants
const KNOTS_TO_METERS_PER_SECOND: f64 = 0.514444;
const NAUTICAL_MILES_TO_METERS: f64 = 1852.0;
const EARTH_RADIUS_METERS: f64 = 6371000.0;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Airplane {
    pub callsign: String,
    pub aircraft_type: String,
    pub latitude: f64,
    pub longitude: f64,
    pub altitude: f64, // in feet
    pub speed: f64,    // in knots
    pub heading: f64,  // degrees from magnetic north
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Alert {
    pub plane1_callsign: String,
    pub plane2_callsign: String,
    pub distance_nm: f64,
    pub altitude_diff_ft: f64,
}

/// Aircraft types for demo data generation
const AIRCRAFT_TYPES: &[&str] = &[
    "A320", "A321", "A330", "A350", "A380",
    "B737", "B747", "B777", "B787",
    "CRJ900", "ERJ190", "ATR72",
    "E175", "E190", "Q400",
];

/// Generate demo airplane data
pub fn generate_demo_airplanes() -> Vec<Airplane> {
    let mut planes = Vec::with_capacity(NUM_DEMO_PLANES + 2);
    let mut rng = rand::rng();
    
    // Add the two required planes for alert detection
    planes.push(Airplane {
        callsign: "REQ001".to_string(),
        aircraft_type: "A320".to_string(),
        latitude: 48.288158,
        longitude: 14.191473,
        altitude: 30000.0,
        speed: 120.0,
        heading: 180.0, // due south
    });
    
    planes.push(Airplane {
        callsign: "REQ002".to_string(),
        aircraft_type: "B737".to_string(),
        latitude: 48.188992,
        longitude: 14.191473,
        altitude: 29500.0,
        speed: 120.0,
        heading: 0.0, // due north
    });
    
    // Generate additional demo planes
    let mut used_callsigns = std::collections::HashSet::new();
    used_callsigns.insert("REQ001".to_string());
    used_callsigns.insert("REQ002".to_string());
    
    for _ in 0..NUM_DEMO_PLANES {
        // Generate unique callsign
        let mut callsign;
        loop {
            callsign = generate_random_callsign(&mut rng);
            if !used_callsigns.contains(&callsign) {
                used_callsigns.insert(callsign.clone());
                break;
            }
        }
        
        // Generate position within 100km radius of LNZ
        let (lat, lng) = generate_random_position_near_lnz(&mut rng);
        
        planes.push(Airplane {
            callsign,
            aircraft_type: AIRCRAFT_TYPES[rng.random_range(0..AIRCRAFT_TYPES.len())].to_string(),
            latitude: lat,
            longitude: lng,
            altitude: rng.random_range(5000.0..40000.0), // 5,000 to 40,000 feet
            speed: rng.random_range(80.0..500.0), // 80 to 500 knots
            heading: rng.random_range(0.0..360.0), // 0 to 359 degrees
        });
    }
    
    planes
}

/// Generate a random callsign with letters and numbers
fn generate_random_callsign(rng: &mut impl Rng) -> String {
    let letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    let digits = "0123456789";
    
    // Format: 3 letters + 3 numbers (e.g., "AAL123")
    let mut callsign = String::with_capacity(6);
    
    for _ in 0..3 {
        let idx = rng.random_range(0..letters.len());
        callsign.push(letters.chars().nth(idx).unwrap());
    }
    
    for _ in 0..3 {
        let idx = rng.random_range(0..digits.len());
        callsign.push(digits.chars().nth(idx).unwrap());
    }
    
    callsign
}

/// Generate a random position within 100km of LNZ airport
fn generate_random_position_near_lnz(rng: &mut impl Rng) -> (f64, f64) {
    // Generate random distance (0-100km) and bearing
    let distance_km = rng.random_range(0.0..100.0);
    let bearing_rad = rng.random_range(0.0..2.0 * std::f64::consts::PI);
    
    // Convert to lat/lng offset
    let distance_m = distance_km * 1000.0;
    let lat_offset = (distance_m * bearing_rad.cos()) / EARTH_RADIUS_METERS * 180.0 / std::f64::consts::PI;
    let lng_offset = (distance_m * bearing_rad.sin()) / (EARTH_RADIUS_METERS * (LNZ_LAT * std::f64::consts::PI / 180.0).cos()) * 180.0 / std::f64::consts::PI;
    
    (LNZ_LAT + lat_offset, LNZ_LNG + lng_offset)
}

/// Calculate updated airplane positions based on elapsed time
pub fn calculate_airplane_positions(planes: &[Airplane], elapsed_seconds: f64) -> Vec<Airplane> {
    planes.iter().map(|plane| {
        let mut updated_plane = plane.clone();
        
        // Convert speed from knots to meters per second
        let speed_ms = plane.speed * KNOTS_TO_METERS_PER_SECOND;
        
        // Calculate distance traveled
        let distance_traveled = speed_ms * elapsed_seconds;
        
        // Convert heading to radians (0° = North, 90° = East)
        let heading_rad = plane.heading * std::f64::consts::PI / 180.0;
        
        // Calculate new position
        let lat_rad = plane.latitude * std::f64::consts::PI / 180.0;
        let lng_rad = plane.longitude * std::f64::consts::PI / 180.0;
        
        // Calculate new latitude
        let new_lat_rad = lat_rad + (distance_traveled * heading_rad.cos()) / EARTH_RADIUS_METERS;
        
        // Calculate new longitude
        let new_lng_rad = lng_rad + (distance_traveled * heading_rad.sin()) / (EARTH_RADIUS_METERS * lat_rad.cos());
        
        // Convert back to degrees
        updated_plane.latitude = new_lat_rad * 180.0 / std::f64::consts::PI;
        updated_plane.longitude = new_lng_rad * 180.0 / std::f64::consts::PI;
        
        updated_plane
    }).collect()
}

/// Check if two airplanes trigger an alert using Haversine formula
pub fn check_alert_between_planes(plane1: &Airplane, plane2: &Airplane) -> Option<Alert> {
    // Calculate distance using Haversine formula
    let distance_nm = haversine_distance(
        plane1.latitude, plane1.longitude,
        plane2.latitude, plane2.longitude
    );
    
    // Calculate altitude difference
    let altitude_diff = (plane1.altitude - plane2.altitude).abs();
    
    // Check if alert conditions are met
    if distance_nm <= ALERT_DISTANCE_NM && altitude_diff < ALERT_ALTITUDE_DIFF_FT {
        Some(Alert {
            plane1_callsign: plane1.callsign.clone(),
            plane2_callsign: plane2.callsign.clone(),
            distance_nm,
            altitude_diff_ft: altitude_diff,
        })
    } else {
        None
    }
}

/// Check all combinations of airplanes for alerts
pub fn check_all_alerts(planes: &[Airplane]) -> Vec<Alert> {
    let mut alerts = Vec::new();
    
    for i in 0..planes.len() {
        for j in (i + 1)..planes.len() {
            if let Some(alert) = check_alert_between_planes(&planes[i], &planes[j]) {
                alerts.push(alert);
            }
        }
    }
    
    alerts
}

/// Calculate distance between two lat/lng points using Haversine formula
/// Returns distance in nautical miles
fn haversine_distance(lat1: f64, lng1: f64, lat2: f64, lng2: f64) -> f64 {
    let lat1_rad = lat1 * std::f64::consts::PI / 180.0;
    let lat2_rad = lat2 * std::f64::consts::PI / 180.0;
    let delta_lat = (lat2 - lat1) * std::f64::consts::PI / 180.0;
    let delta_lng = (lng2 - lng1) * std::f64::consts::PI / 180.0;
    
    let a = (delta_lat / 2.0).sin().powi(2) +
            lat1_rad.cos() * lat2_rad.cos() *
            (delta_lng / 2.0).sin().powi(2);
    
    let c = 2.0 * a.sqrt().atan2((1.0 - a).sqrt());
    
    // Distance in meters
    let distance_m = EARTH_RADIUS_METERS * c;
    
    // Convert to nautical miles
    distance_m / NAUTICAL_MILES_TO_METERS
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_haversine_distance() {
        // Test with known coordinates - the two required planes from the spec
        let distance = haversine_distance(48.288158, 14.191473, 48.188992, 14.191473);

        // These two points are about 5.95 nautical miles apart
        assert!((distance - 5.95).abs() < 0.1);
    }

    #[test]
    fn test_alert_detection() {
        // Create two planes that are close enough to trigger an alert
        let plane1 = Airplane {
            callsign: "TEST1".to_string(),
            aircraft_type: "A320".to_string(),
            latitude: 48.250000,
            longitude: 14.191473,
            altitude: 30000.0,
            speed: 120.0,
            heading: 180.0,
        };
        
        let plane2 = Airplane {
            callsign: "TEST2".to_string(),
            aircraft_type: "B737".to_string(),
            latitude: 48.265000, // About 1 nm apart
            longitude: 14.191473,
            altitude: 29500.0, // 500 ft altitude difference
            speed: 120.0,
            heading: 0.0,
        };
        
        // These planes should trigger an alert (close distance, small altitude diff)
        let alert = check_alert_between_planes(&plane1, &plane2);
        assert!(alert.is_some());
        
        if let Some(alert) = alert {
            assert_eq!(alert.plane1_callsign, "TEST1");
            assert_eq!(alert.plane2_callsign, "TEST2");
            assert!(alert.distance_nm <= ALERT_DISTANCE_NM);
            assert!(alert.altitude_diff_ft < ALERT_ALTITUDE_DIFF_FT);
        }
    }

    #[test]
    fn test_generate_demo_airplanes() {
        let planes = generate_demo_airplanes();
        assert_eq!(planes.len(), NUM_DEMO_PLANES + 2);
        
        // Check that required planes are included
        assert!(planes.iter().any(|p| p.callsign == "REQ001"));
        assert!(planes.iter().any(|p| p.callsign == "REQ002"));
        
        // Check all callsigns are unique
        let mut callsigns: Vec<_> = planes.iter().map(|p| &p.callsign).collect();
        callsigns.sort();
        callsigns.dedup();
        assert_eq!(callsigns.len(), planes.len());
    }
}

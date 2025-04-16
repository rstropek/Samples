use wasm_bindgen::prelude::*;

use crate::helpers::Point2d;

#[wasm_bindgen]
pub struct Radar {
    /// Location of the radar station
    pub center: Point2d,

    /// Radius of the reach of the radar station
    pub reach_radius: f64,

    /// Current angle of the radar beam (0 is pointing up)
    pub current_angle: f64,    

    /// Width of the radar beam
    /// 
    /// A value of 5 would mean that the beam detects objects within 2.5 degrees 
    /// on either side of the current angle
    pub beam_width_angle: f64
}

impl Default for Radar {
    fn default() -> Self {
        Self { 
            center: Point2d::new(500.0, 500.0), 
            reach_radius: 400.0, 
            current_angle: 0.0, 
            beam_width_angle: 10.0
        }
    }
}

#[wasm_bindgen]
impl Radar {
    pub fn new() -> Self {
        Self::default()
    }

    pub fn update(&mut self) {
        self.current_angle += 0.1;
    }

    pub fn can_detect(&self, object: &Point2d) -> bool {
        // Calculate the vector from radar center to object
        let dx = object.x - self.center.x;
        let dy = object.y - self.center.y;
        let distance = (dx * dx + dy * dy).sqrt();
        
        // Check if object is within radar's reach radius
        if distance <= self.reach_radius {
            // Calculate angle from radar to object (in radians)
            // Angle 0 is pointing up (negative y-axis in screen coordinates)
            let object_angle = (dx.atan2(-dy) * 180.0 / std::f64::consts::PI) % 360.0;
            
            // Normalize angles to 0-360 degrees
            let radar_angle = self.current_angle % 360.0;
            let half_beam_width = self.beam_width_angle / 2.0;
            
            // Calculate the minimum difference between angles (considering the circular nature)
            let mut angle_diff = (object_angle - radar_angle).abs();
            if angle_diff > 180.0 {
                angle_diff = 360.0 - angle_diff;
            }
            
            // Check if object is within radar's beam width
            return angle_diff <= half_beam_width;
        }
        
        false
    }
}
#![allow(unused)]

use std::time::Duration;

#[cfg(test)]
use mock_instant::global::Instant;
use serde::Serialize;

#[cfg(not(test))]
use std::time::Instant;

pub struct RotatingDiskSimulator {
    radius: f64,
    angular_velocity: f64,
    t0: Instant
}

#[derive(Debug, Clone, Copy, Serialize)]
pub struct Measurement {
    pub timestamp: f64,
    pub dx: f64,
    pub dy: f64
}

impl RotatingDiskSimulator {
    fn new(radius: f64, angular_velocity: f64) -> Self {
        RotatingDiskSimulator {
            radius,
            angular_velocity,
            t0: Instant::now()
        }
    }
}

pub trait RotationSensor {
    fn measure(&self) -> Measurement;
}

impl RotationSensor for RotatingDiskSimulator {
    fn measure(&self) -> Measurement {
        let t = self.t0.elapsed().as_secs_f64();
        let angle = self.angular_velocity * t;
        let dx = self.radius * angle.cos();
        let dy = self.radius * angle.sin();
        Measurement { timestamp: t, dx, dy }
    }
}

pub struct RotatingDiskSimulatorBuilder {
    pub radius: f64,
    pub rotation_duraction: Duration
}

impl Default for RotatingDiskSimulatorBuilder {
    fn default() -> Self {
        Self {
            radius: 5.0,
            rotation_duraction: Duration::from_secs(10)
        }
    }
}

impl RotatingDiskSimulatorBuilder {
    pub fn new() -> Self {
        Self::default()
    }

    pub fn with_radius(mut self, radius: f64) -> Self {
        self.radius = radius;
        self
    }

    pub fn with_rotation_duration(mut self, duration: Duration) -> Self {
        self.rotation_duraction = duration;
        self
    }

    pub fn build(self) -> RotatingDiskSimulator {
        RotatingDiskSimulator::new(self.radius, 2.0 * std::f64::consts::PI / self.rotation_duraction.as_secs_f64())
    }
}

#[cfg(test)]
mod tests {
    use mock_instant::global::MockClock;

    use super::*;

    #[test]
    fn test_builder_default() {
        let builder = RotatingDiskSimulatorBuilder::new();
        assert_eq!(builder.radius, 5.0);
        assert_eq!(builder.rotation_duraction, Duration::from_secs(10));
    }

    #[test]
    fn test_builder_with_radius() {
        let builder = RotatingDiskSimulatorBuilder::new()
            .with_radius(10.0);
        assert_eq!(builder.radius, 10.0);
        assert_eq!(builder.rotation_duraction, Duration::from_secs(10)); // default duration
    }

    #[test]
    fn test_builder_with_rotation_duration() {
        let builder = RotatingDiskSimulatorBuilder::new()
            .with_rotation_duration(Duration::from_secs(5));
        assert_eq!(builder.radius, 5.0); // default radius
        assert_eq!(builder.rotation_duraction, Duration::from_secs(5));
    }

    #[test]
    fn test_builder_build() {
        let radius = 8.0;
        let duration = Duration::from_secs(4);
        let simulator = RotatingDiskSimulatorBuilder::new()
            .with_radius(radius)
            .with_rotation_duration(duration)
            .build();
        
        // Test initial measurement
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, 8.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, 0.0);
    }

    #[test]
    fn test_measure() {
        // Create simulator with r = 1 and rotation duration = 1
        let simulator = RotatingDiskSimulatorBuilder::new()
            .with_radius(1.0)
            .with_rotation_duration(Duration::from_secs(1))
            .build();
        
        // Test initial measurement
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, 1.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, 0.0);

        // Test measurement after 250ms
        MockClock::advance(Duration::from_millis(250));
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, 0.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, 1.0);

        // Test measurement after 500ms
        MockClock::advance(Duration::from_millis(250));
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, -1.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, 0.0);

        // Test measurement after 750ms
        MockClock::advance(Duration::from_millis(250));
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, 0.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, -1.0);

        // Test measurement after 1000ms
        MockClock::advance(Duration::from_millis(250));
        assert_eq!((simulator.measure().dx * 100.0).round() / 100.0, 1.0);
        assert_eq!((simulator.measure().dy * 100.0).round() / 100.0, 0.0);
    }
}
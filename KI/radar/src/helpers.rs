use std::ops::{Add, AddAssign};

use wasm_bindgen::prelude::wasm_bindgen;

#[wasm_bindgen]
#[derive(Clone, Default, Copy)]
pub struct Vector2d {
    pub x: f64,
    pub y: f64,
}

impl Vector2d {
    pub fn new(x: f64, y: f64) -> Self {
        Self { x, y }
    }
}

impl AddAssign for Vector2d {
    fn add_assign(&mut self, other: Self) {
        *self = Self {
            x: self.x + other.x,
            y: self.y + other.y,
        };
    }
}

#[wasm_bindgen]
#[derive(Clone, Default, Copy)]
pub struct Point2d {
    pub x: f64,
    pub y: f64,
}

impl Point2d {
    pub fn new(x: f64, y: f64) -> Self {
        Self { x, y }
    }
}

impl Add<Vector2d> for Point2d {
    type Output = Point2d;

    fn add(self, other: Vector2d) -> Point2d {
        Point2d { x: self.x + other.x, y: self.y + other.y }
    }
}

impl AddAssign<Vector2d> for Point2d {
    fn add_assign(&mut self, other: Vector2d) {
        *self = *self + other;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_vector2d_add_assign() {
        let mut v1 = Vector2d::new(1.0, 2.0);
        let v2 = Vector2d::new(3.0, 4.0);
        
        v1 += v2;
        
        assert_eq!(v1.x, 4.0);
        assert_eq!(v1.y, 6.0);
    }

    #[test]
    fn test_point2d_add_assign_vector() {
        let mut p = Point2d::new(5.0, 6.0);
        let v = Vector2d::new(2.0, 3.0);
        
        p += v;
        
        assert_eq!(p.x, 7.0);
        assert_eq!(p.y, 9.0);
    }
}

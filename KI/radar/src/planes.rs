use js_sys::Math::random;
use wasm_bindgen::prelude::*;

use crate::helpers::{Point2d, Vector2d};

fn random_range(min: f64, max: f64) -> f64 {
    min + (max - min) * random()
}

#[wasm_bindgen]
#[derive(Clone)]
pub struct Plane {
    pub position: Point2d,
    pub velocity: Vector2d,
}

impl Plane {
    pub fn new_random() -> Self {
        let position = Point2d::new(random_range(0.0, 1000.0), random_range(0.0, 1000.0));
        let velocity = Vector2d::new(random_range(-5.0, 5.0), random_range(-5.0, 5.0));
        Self { position, velocity }
    }
}

#[wasm_bindgen]
pub struct Planes {
    planes: Vec<Plane>,
}

#[wasm_bindgen]
impl Planes {
    pub fn new() -> Self {
        let mut planes = Vec::new();
        for _ in 0..10 {
            planes.push(Plane::new_random());
        }
        Self { planes }
    }

    pub fn update(&mut self) {
        for plane in self.planes.iter_mut() {
            plane.position += plane.velocity;
        }
    }

    pub fn number_of_planes(&self) -> usize {
        self.planes.len()
    }

    pub fn get_plane(&self, index: usize) -> Plane {
        self.planes[index].clone()
    }
}
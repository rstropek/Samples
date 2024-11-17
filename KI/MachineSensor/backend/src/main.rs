use std::{sync::Arc, time::Duration};

use axum::{extract::State, routing::{get, post}, Json, Router};
use ring_buffer::RingBuffer;
use sensor::{Measurement, RotatingDiskSimulatorBuilder, RotationSensor};
use tokio::sync::RwLock;

mod sensor;
mod ring_buffer;

struct AppState {
    measurement_history: RwLock<RingBuffer<Measurement>>,
    sensor: Box<dyn RotationSensor + Send + Sync>
}

#[tokio::main]
async fn main() {
    let sensor = RotatingDiskSimulatorBuilder::new()
        .with_radius(5.0)
        .with_rotation_duration(Duration::from_secs(5))
        .build();
    
    let state = Arc::new(AppState{
        measurement_history: RwLock::new(RingBuffer::new(50)),
        sensor: Box::new(sensor)
    });

    let state_clone = state.clone();
    tokio::spawn(async move {
        loop {
            let m = state_clone.sensor.measure();
            state_clone.measurement_history.write().await.push(m);            
            tokio::time::sleep(Duration::from_millis(100)).await;
        }
    });
    
    // build our application with a route
    let app = Router::new()
        .route("/ping", get(|| async { Json("pong") }))
        .route("/measurements", get(get_measurements))
        .route("/measurement/measure-now", post(measure))
        .with_state(state.clone())
        .with_state(state);

    // run it
    let listener = tokio::net::TcpListener::bind("127.0.0.1:3000")
        .await
        .unwrap();
    println!("listening on {}", listener.local_addr().unwrap());
    axum::serve(listener, app)
        .await
        .unwrap();
}

async fn get_measurements(state: State<Arc<AppState>>) -> Json<Vec<Measurement>> {
    let measurements: Vec<Measurement> = state.measurement_history.read().await.into_iter().collect();
    Json(measurements)
}

async fn measure(sensor: State<Arc<AppState>>) -> Json<Measurement> {
    let measurement = sensor.sensor.measure();
    Json(measurement)
}

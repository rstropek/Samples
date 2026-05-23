use std::{
    convert::Infallible,
    sync::atomic::{AtomicU64, Ordering},
    time::Duration,
};

use axum::{
    Router,
    response::{Sse, sse::Event},
    routing::get,
};
use futures_util::stream::Stream;
use once_cell::sync::Lazy;
use serde::Serialize;
use tokio::{net::TcpListener, time::interval};
use tokio_stream::StreamExt as _;
use tower_http::cors::{Any, CorsLayer};

use crate::planes::{calculate_airplane_positions, check_all_alerts, generate_demo_airplanes, Airplane, Alert};

mod planes;

static PLANES: Lazy<Vec<Airplane>> = Lazy::new(|| generate_demo_airplanes());
static COUNTER: AtomicU64 = AtomicU64::new(0);

#[derive(Serialize)]
struct EventData {
    planes: Vec<Airplane>,
    alerts: Vec<Alert>,
}

#[tokio::main]
async fn main() {
    // Spawn background task to increment counter every second
    tokio::spawn(async {
        let mut interval = interval(Duration::from_secs(1));
        loop {
            interval.tick().await;
            COUNTER.fetch_add(1, Ordering::Relaxed);
        }
    });

    let router = Router::new().route("/sse", get(sse_handler))
        .layer(CorsLayer::new().allow_methods(Any).allow_headers(Any).allow_origin(Any));

    let listener = TcpListener::bind("127.0.0.1:3000").await.unwrap();
    axum::serve(listener, router).await.unwrap();
}

async fn sse_handler() -> Sse<impl Stream<Item = Result<Event, Infallible>>> {
    let stream = futures_util::stream::repeat_with(|| {
        let counter_value = COUNTER.load(Ordering::Relaxed);
        let planes = calculate_airplane_positions(&PLANES, counter_value as f64);
        let alerts = check_all_alerts(&planes);
        let event_data = EventData {
            planes,
            alerts,
        };
        Event::default()
            .json_data(event_data)
            .unwrap()
    })
    .map(Ok)
    .throttle(Duration::from_secs(1));

    Sse::new(stream).keep_alive(
        axum::response::sse::KeepAlive::new()
            .interval(Duration::from_secs(1))
            .text("keep-alive"),
    )
}

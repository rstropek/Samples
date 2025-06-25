use std::{convert::Infallible, sync::Arc};

use axum::{response::Sse, routing::get, Router};
use tokio::{net::TcpListener, stream, sync::RwLock};
use tokio_stream::Stream;

mod planes;

type Planes = Arc<RwLock<Vec<Plane>>>;

#[tokio::main]
async fn main() {
    let router = Router::new()/*
        .route("/sse", get(sse_handler))*/;

    let listener = TcpListener::bind("*:3000")
        .await
        .unwrap();
    axum::serve(listener, router).await.unwrap();
}
/*

async fn sse_handler() -> Sse<impl Stream<Item = Result<Event, Infallible>>> {
    let stream = stream::repeat_with(|| Event::default().data("hi!"))
        .map(Ok)
        .throttle(Duration::from_secs(1));

    Sse::new(stream).keep_alive(
        axum::response::sse::KeepAlive::new()
            .interval(Duration::from_secs(1))
            .text("keep-alive-text"),
    )
}
    */
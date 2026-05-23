use crate::app_error::AppError;
use askama::Template;
use async_openai::{
    Client,
    error::{OpenAIError, StreamError},
    types::{
        ReasoningEffort,
        responses::{CreateResponseArgs, ReasoningConfigArgs, ResponseEvent},
    },
};
use axum::{
    Router,
    response::{
        Html, IntoResponse,
        sse::{Event, Sse},
    },
    routing::get,
};
use futures::StreamExt;
use futures_util::stream::Stream;
use std::convert::Infallible;
use tokio_stream::wrappers::ReceiverStream;

mod app_error;

#[tokio::main]
async fn main() {
    dotenvy::dotenv().ok();

    let app = Router::new()
        .route("/", get(index))
        .route("/ping", get(handler))
        .route("/chat", get(chat_handler));

    let listener = tokio::net::TcpListener::bind("127.0.0.1:3000")
        .await
        .unwrap();
    println!("listening on http://{}", listener.local_addr().unwrap());
    axum::serve(listener, app).await.unwrap();
}

async fn handler() -> &'static str {
    "pong"
}

async fn index() -> Result<impl IntoResponse, AppError> {
    #[derive(Template)]
    #[template(path = "index.html")]
    struct IndexTemplate();

    let template = IndexTemplate {};
    Ok(Html(template.render().unwrap()))
}

async fn chat_handler() -> Sse<impl Stream<Item = Result<Event, Infallible>>> {
    // Create a channel to send events
    let (tx, rx) = tokio::sync::mpsc::channel(10);

    // Spawn a task that will generate and send events
    tokio::spawn(async move {
        let client = Client::new();

        let request = CreateResponseArgs::default()
            .model("gpt-5")
            .stream(true)
            .reasoning(
                ReasoningConfigArgs::default()
                    .effort(ReasoningEffort::Minimal)
                    .build()
                    .unwrap(),
            )
            .instructions("You are a helpful assistant")
            .input("Are Dolphins fish?")
            .build()
            .unwrap();

        let mut stream = client.responses().create_stream(request).await.unwrap();

        while let Some(result) = stream.next().await {
            match result {
                Ok(ResponseEvent::ResponseOutputTextDelta(delta)) => {
                    let _ = tx
                        .send(Ok(Event::default().json_data(delta.delta.clone()).unwrap()))
                        .await;
                }
                Ok(_) => {}
                Err(OpenAIError::StreamError(StreamError::ReqwestEventSource(
                    reqwest_eventsource::Error::StreamEnded,
                ))) => {
                    // Stream ended gracefully
                    break;
                }
                Err(e) => {
                    eprintln!("{e:#?}");
                }
            }
        }

        // Indicate that the stream is done to the client
        let _ = tx
            .send(Ok(Event::default().json_data("[DONE]").unwrap()))
            .await;
    });

    Sse::new(ReceiverStream::new(rx))
}

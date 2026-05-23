use rmcp::{
    ServerHandler,
    handler::server::{tool::ToolRouter, wrapper::Parameters},
    model::{ServerCapabilities, ServerInfo},
    tool, tool_handler, tool_router,
    transport::{
        StreamableHttpService, streamable_http_server::session::local::LocalSessionManager,
    },
};
use schemars::JsonSchema;

mod password_generator;

const BIND_ADDRESS: &str = "127.0.0.1:8000";

#[tokio::main]
async fn main() -> anyhow::Result<()> {
    let service = StreamableHttpService::new(
        || Ok(PonyPasswordGenerator::new()),
        LocalSessionManager::default().into(),
        Default::default(),
    );

    let router = axum::Router::new().nest_service("/mcp", service);
    let tcp_listener = tokio::net::TcpListener::bind(BIND_ADDRESS).await?;
    let _ = axum::serve(tcp_listener, router)
        .with_graceful_shutdown(async { tokio::signal::ctrl_c().await.unwrap() })
        .await;
    Ok(())
}

struct PonyPasswordGenerator {
    tool_router: ToolRouter<Self>,
}

#[derive(Debug, serde::Deserialize, JsonSchema)]
struct PasswordParameters {
    pub min_length: usize,
}

#[tool_router]
impl PonyPasswordGenerator {
    pub fn new() -> Self {
        Self {
            tool_router: Self::tool_router(),
        }
    }

    #[tool(
        description = "Generates a password by concatenating character names from My Little Pony."
    )]
    fn generate_password(
        &self,
        Parameters(PasswordParameters { min_length }): Parameters<PasswordParameters>,
    ) -> String {
        let password = password_generator::generate_pony_password(min_length);
        password
    }
}

#[tool_handler]
impl ServerHandler for PonyPasswordGenerator {
    fn get_info(&self) -> ServerInfo {
        ServerInfo {
            instructions: Some(
                "Generates a password by concatenating character names from My Little Pony.".into(),
            ),
            capabilities: ServerCapabilities::builder().enable_tools().build(),
            ..Default::default()
        }
    }
}

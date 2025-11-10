use anyhow::Result;
use rig::{agent::stream_to_stdout, providers::openai};
use rig::prelude::*;
use schemars::{schema_for, JsonSchema};

use rig::{completion::ToolDefinition, providers, streaming::StreamingPrompt, tool::Tool};
use serde::{Deserialize, Serialize};
use serde_json::json;

#[derive(Deserialize, JsonSchema)]
struct OperationArgs {
    x: i32,
    y: i32,
}

#[derive(Debug, thiserror::Error)]
#[error("Math error")]
struct MathError;

#[derive(Deserialize, Serialize)]
struct Adder;

impl Tool for Adder {
    const NAME: &'static str = "add";
    type Error = MathError;
    type Args = OperationArgs;
    type Output = i32;

    async fn definition(&self, _prompt: String) -> ToolDefinition {
        ToolDefinition {
            name: "add".to_string(),
            description: "Add x and y together".to_string(),
            parameters: schema_for!(OperationArgs).into(),
        }
    }

    async fn call(&self, args: Self::Args) -> Result<Self::Output, Self::Error> {
        let result = args.x + args.y;
        Ok(result)
    }
}

#[derive(Deserialize, Serialize)]
struct Subtract;

impl Tool for Subtract {
    const NAME: &'static str = "subtract";
    type Error = MathError;
    type Args = OperationArgs;
    type Output = i32;

    async fn definition(&self, _prompt: String) -> ToolDefinition {
        serde_json::from_value(json!({
            "name": "subtract",
            "description": "Subtract y from x (i.e.: x - y)",
            "parameters": {
                "type": "object",
                "properties": {
                    "x": {
                        "type": "number",
                        "description": "The number to subtract from"
                    },
                    "y": {
                        "type": "number",
                        "description": "The number to subtract"
                    }
                },
                "required": ["x", "y"],
            }
        }))
        .expect("Tool Definition")
    }

    async fn call(&self, args: Self::Args) -> Result<Self::Output, Self::Error> {
        let result = args.x - args.y;
        Ok(result)
    }
}

#[derive(Serialize, Deserialize, JsonSchema)]
enum Operation {
    Add,
    Subtract
}

#[derive(Serialize, Deserialize, JsonSchema)]
struct TriageResult {
    operation: Operation
}

#[tokio::main]
async fn main() -> Result<(), anyhow::Error> {
    dotenvy::dotenv().ok();

    let openai_client = openai::Client::from_env();

    let add_agent = openai_client
        .agent("gpt-5")
        .additional_params(json!({ "reasoning": { "effort": "minimal"} }))
        .preamble(
            "You are a calculator here to help the user perform add
            operations. Use the tools provided to answer the user's question.
            Include a brief explanation of how you arrived at the answer.
            Add your name ('Adder') to the beginning of your answer.",
        )
        .tool(Adder)
        .build();

    let sub_agent = openai_client
        .agent(providers::openai::GPT_4_1)
        .preamble(
            "You are a calculator here to help the user perform subtract
            operations. Use the tools provided to answer the user's question.
            Add your name ('Subtracter') to the beginning of your answer.",
        )
        .tool(Subtract)
        .build();

    let triage_agent = openai_client
        .extractor::<TriageResult>(openai::GPT_4_1_MINI)
        .build();

    const QUESTION: &str = "Please add the numbers 22 and 20";

    let calculator_agent = match triage_agent
        .extract(QUESTION).await.unwrap()
        .operation {
            Operation::Add => add_agent,
            Operation::Subtract => sub_agent,
        };

    let mut stream = calculator_agent.stream_prompt(QUESTION).await;

    let _ = stream_to_stdout(&mut stream).await?;

    Ok(())
}
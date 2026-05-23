# Standard library imports
from typing import AsyncGenerator
import os
import json

# Third-party imports
from dotenv import load_dotenv
from fastapi import FastAPI, Request
from fastapi.responses import StreamingResponse, HTMLResponse, FileResponse
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from openai.types.responses import ResponseTextDeltaEvent
from agents import Agent, InputGuardrail, Runner, InputGuardrailTripwireTriggered
from agents.mcp import MCPServer, MCPServerStdio
import uvicorn

# Local imports
from guardrails_agents import coding_guardrail
from python_tutor import python_tutor
from readme_generator import readme_generator
from time_cockpit_mcp import get_time_cockpit_agent

# Load environment variables
load_dotenv()

def run():
    triage_agent = Agent(
        name="Triage Agent",
        instructions="""
            You determine which agent to use based on the user's coding-related question.
            """,
        handoffs=[python_tutor, readme_generator],
        input_guardrails=[
            InputGuardrail(guardrail_function=coding_guardrail),
        ],
    )

    async def process_input(user_input) -> AsyncGenerator[str, None]:
        try:
            result = Runner.run_streamed(triage_agent, user_input)
            async for event in result.stream_events():
                if event.type == "raw_response_event" and isinstance(event.data, ResponseTextDeltaEvent):
                    event_json = json.dumps(event.data.delta)
                    yield f"data: {event_json}\n\n"
            
            # Add [END] marker when the response is complete
            yield f"data: \"[END]\"\n\n"
        except InputGuardrailTripwireTriggered as e:
            # Create an async generator for the error message
            error_json = json.dumps(f'\n\nGuardrail triggered: {e.guardrail_result.output.output_info.reasoning}')
            yield f"data: {error_json}\n\n"
            yield f"data: \"[END]\"\n\n"  # Add [END] marker
        except Exception as e:
            # Handle exceptions that occur during streaming
            error_json = json.dumps(f"\n\nError during streaming: {str(e)}")
            yield f"data: {error_json}\n\n"
            yield f"data: \"[END]\"\n\n"

    app = FastAPI()

    # Mount static files directory
    BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    STATIC_DIR = os.path.join(BASE_DIR, "static")
    app.mount("/static", StaticFiles(directory=STATIC_DIR), name="static")

    # Add CORS middleware to allow all origins
    app.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_methods=["*"],
        allow_headers=["*"],
    )

    @app.get("/", response_class=HTMLResponse)
    async def get_html():
        return FileResponse(os.path.join(STATIC_DIR, "index.html"))

    @app.get("/chat")
    async def chat(request: Request):
        # Get query string parameter "user_input"
        user_input = request.query_params.get("user_input")
        return StreamingResponse(process_input(user_input), media_type="text/event-stream")

    return app

if __name__ == "__main__":
    uvicorn.run(run(), host="0.0.0.0", port=8000)

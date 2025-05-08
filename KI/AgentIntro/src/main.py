# Standard library imports
import asyncio
from typing import AsyncGenerator
import os

# Third-party imports
from dotenv import load_dotenv
from fastapi import FastAPI, Request
from fastapi.responses import StreamingResponse, HTMLResponse, FileResponse
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from openai.types.responses import ResponseTextDeltaEvent
from pydantic import BaseModel
from agents import Agent, InputGuardrail, GuardrailFunctionOutput, Runner, InputGuardrailTripwireTriggered

# Load environment variables
load_dotenv()

# Get the current directory
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
STATIC_DIR = os.path.join(BASE_DIR, "static")

class HomeworkOutput(BaseModel):
    is_homework: bool
    reasoning: str

guardrail_agent = Agent(
    name="Guardrail check",
    instructions="Check if the user is asking about homework.",
    output_type=HomeworkOutput,
)

math_tutor_agent = Agent(
    name="Math Tutor",
    handoff_description="Specialist agent for math questions",
    instructions="You provide help with math problems. Explain your reasoning at each step and include examples",
)

history_tutor_agent = Agent(
    name="History Tutor",
    handoff_description="Specialist agent for historical questions",
    instructions="You provide assistance with historical queries. Explain important events and context clearly.",
)


async def homework_guardrail(ctx, agent, input_data):
    result = await Runner.run(guardrail_agent, input_data, context=ctx.context)
    final_output = result.final_output_as(HomeworkOutput)
    return GuardrailFunctionOutput(
        output_info=final_output,
        tripwire_triggered=not final_output.is_homework,
    )

triage_agent = Agent(
    name="Triage Agent",
    instructions="You determine which agent to use based on the user's homework question",
    handoffs=[history_tutor_agent, math_tutor_agent],
    input_guardrails=[
        InputGuardrail(guardrail_function=homework_guardrail),
    ],
)

async def process_input(user_input) -> AsyncGenerator[str, None]:
    try:
        result = Runner.run_streamed(triage_agent, user_input)
        async for event in result.stream_events():
            if event.type == "raw_response_event" and isinstance(event.data, ResponseTextDeltaEvent):
                yield f"data: {event.data.delta}\n\n"
        
        # Add [END] marker when the response is complete
        yield f"data: [END]\n\n"
    except InputGuardrailTripwireTriggered as e:
        # Create an async generator for the error message
        yield f"data: Guardrail triggered: {e.guardrail_result.output.output_info.reasoning}\n\n"
        yield f"data: [END]\n\n"  # Add [END] marker
    except Exception as e:
        # Handle exceptions that occur during streaming
        yield f"data: Error during streaming: {str(e)}\n\n"
        yield f"data: [END]\n\n"

app = FastAPI()

# Mount static files directory
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

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)

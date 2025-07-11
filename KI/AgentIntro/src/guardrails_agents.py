from pydantic import BaseModel
from agents import Agent, GuardrailFunctionOutput, Runner, InputGuardrailTripwireTriggered

class CodingOutput(BaseModel):
    is_coding: bool
    reasoning: str

guardrail_agent = Agent(
    model="gpt-4o-mini",
    name="Guardrail check",
    instructions="""
        Check if the user asks coding-related questions.
        """,
    output_type=CodingOutput,
)

async def coding_guardrail(ctx, agent, input_data):
    result = await Runner.run(guardrail_agent, input_data, context=ctx.context)
    final_output = result.final_output_as(CodingOutput)
    return GuardrailFunctionOutput(
        output_info=final_output,
        tripwire_triggered=not final_output.is_coding,
    )

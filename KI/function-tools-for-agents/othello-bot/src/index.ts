import OpenAI from "openai";
import fs from "fs";
import { getValidMovesTool, handleFunctionCall, resetBoardTool, showBoardTool, tryApplyMoveTool } from "./functions.js";
import type { ResponseInputItem } from "openai/resources/responses/responses.mjs";
import { OthelloBoard } from "othello-game";
import { readLine } from "./input-helper.js";

const client = new OpenAI();

const systemPrompt = await fs.promises.readFile("system-prompt.md", {
  encoding: "utf-8",
});

const board = OthelloBoard.createEmpty();

let previousResponseId: string | null = null;

while (true) {
  const userMessage = await readLine("You:\n");
  console.log();

  const response = createResponse(client, userMessage);
  for await (const event of response) {
    process.stdout.write(event);
  }
  process.stdout.write("\n");
}

async function* createResponse(client: OpenAI, userMessage: string): AsyncGenerator<string> {
  let input: ResponseInputItem[] = [{ role: "user", content: userMessage }];
  let requiresAction: boolean;
  do {
    requiresAction = false;
    let response = await client.responses.create({
      model: "gpt-5",
      input,
      instructions: systemPrompt,
      tool_choice: "auto",
      reasoning: {
        effort: "minimal",
      },
      tools: [resetBoardTool, getValidMovesTool, tryApplyMoveTool, showBoardTool],
      store: true,
      stream: true,
      previous_response_id: previousResponseId,
    });

    input = [];
    for await (const event of response) {
      if (event.type === "response.created") {
        previousResponseId = event.response.id;
      } else if (event.type === "response.output_text.delta") {
        yield event.delta;
      } else if (event.type === "response.output_item.done" && event.item.type !== "reasoning") {
        if (event.item.type === "function_call") {
          writeLineInLightGray(`> Calling function: ${event.item.name}(${JSON.stringify(event.item.arguments)})`);
          requiresAction = true;
          const result = await handleFunctionCall(event.item, board);
          if (result.displayOutput) {
            yield* result.displayOutput;
          }
          writeLineInLightGray(`> Result: ${JSON.stringify(result.functionResult)}`);
          input.push(result.functionResult);
        }
      } else if (event.type === "response.completed") {
        writeLineInLightGray(`> Response completed (${JSON.stringify(event.response.usage)})`);
      }
    }
  } while (requiresAction);
}

function writeLineInLightGray(line: string) {
  process.stdout.write(`\n\x1b[90m${line}\x1b[0m\n`);
}

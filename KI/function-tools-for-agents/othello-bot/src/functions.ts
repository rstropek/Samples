import type { FunctionTool, ResponseCustomToolCallOutput } from "openai/resources/responses/responses.mjs";
import type { OthelloBoard } from "othello-game";
import { zodToJsonSchema } from "zod-to-json-schema";
import { z } from "zod/v3";

export const EmptyObjectSchema = z.object({});

export const resetBoardTool: FunctionTool = {
  type: "function",
  name: "resetBoard",
  description: "Resets the current Othello board to the initial state.",
  parameters: zodToJsonSchema(EmptyObjectSchema),
  strict: true,
};

export const getValidMovesTool: FunctionTool = {
  type: "function",
  name: "getValidMoves",
  description: `Gets the valid moves given the current board state and the player to move as well as the current 
     board state and game statistics (which player has how many stones). Will return an array with valid 
     moves. For each valid move, it will also return the stones that would be flipped with that move.`,
  parameters: zodToJsonSchema(EmptyObjectSchema),
  strict: true,
};

export const PositionSchema = z.object({
  row: z.number().min(0).max(7),
  col: z.number().min(0).max(7),
});
export type Position = z.infer<typeof PositionSchema>;

export const tryApplyMoveTool: FunctionTool = {
  type: "function",
  name: "tryApplyMove",
  description:
    "Tries to apply a move given the current board state and the player to move. Row and column are 0-7. Will return a boolean indicating if the move was successful. Use the getBoard function to get the current board state after the move.",
  parameters: zodToJsonSchema(PositionSchema),
  strict: true,
};

export const showBoardTool: FunctionTool = {
  type: "function",
  name: "showBoard",
  description: "Shows the current board state to the user.",
  parameters: zodToJsonSchema(EmptyObjectSchema),
  strict: true,
};

type FunctionCallResult = {
  functionResult: ResponseCustomToolCallOutput;
  displayOutput: Generator<string> | null;
};

export async function handleFunctionCall(
  item: { name: string; call_id: string; arguments: string },
  board: OthelloBoard,
): Promise<FunctionCallResult> {
  let functionResult: ResponseCustomToolCallOutput;
  let displayOutput: Generator<string> | null = null;

  switch (item.name) {
    case resetBoardTool.name:
      board.reset();
      functionResult = {
        type: "custom_tool_call_output",
        call_id: item.call_id,
        output: "ok",
      };
      break;
    case getValidMovesTool.name:
      const moves = board.getValidMoves();
      const boardWithMoves = {
        ...moves,
        board: board.toString(),
        currentPlayer: board.getCurrentPlayer(),
        stats: board.getGameStatistics(),
      };
      functionResult = {
        type: "custom_tool_call_output",
        call_id: item.call_id,
        output: JSON.stringify(boardWithMoves),
      };
      break;
    case tryApplyMoveTool.name:
      let position: Position;
      try {
        position = await PositionSchema.parseAsync(JSON.parse(item.arguments));
      } catch (error) {
        functionResult = {
          type: "custom_tool_call_output",
          call_id: item.call_id,
          output: `ERROR: ${error}`,
        };
        break;
      }
      functionResult = {
        type: "custom_tool_call_output",
        call_id: item.call_id,
        output: board.tryApplyMove(position) ? "ok" : "Invalid move",
      };
      break;
    case showBoardTool.name:
      displayOutput = (function* () {
        yield "\n\n";
        // Send clear screen
        yield "\x1b[2J\x1b[H";
        yield board.toFormattedString();
        yield "\n";
      })();
      functionResult = {
        type: "custom_tool_call_output",
        call_id: item.call_id,
        output: `ok`,
      };
      break;
    default:
      functionResult = {
        type: "custom_tool_call_output",
        call_id: item.call_id,
        output: `ERROR: Unknown function call: ${item.name}`,
      };
      break;
  }

  return { functionResult, displayOutput };
}

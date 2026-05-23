export type Player = "B" | "W";

export type Position = {
  row: number;
  col: number;
};

export type Move = {
  position: Position;
  flippedPositions: Position[];
};

export type ValidMovesResult = {
  moves: Move[];
};

export type GameStatistics = {
  black: number;
  white: number;
};

export type InvalidBoardResult = {
  error: string;
};

export const isInvalidBoardResult = (input: unknown): input is InvalidBoardResult =>
  typeof input === "object" && input !== null && typeof (input as { error?: unknown }).error === "string";

export class OthelloBoard {
  private board: string[][];
  private currentPlayer: Player;
  private static readonly BOARD_SIZE = 8;

  private constructor(board: string[][], currentPlayer: Player = "B") {
    this.board = board;
    this.currentPlayer = currentPlayer;
  }

  public static createEmpty(): OthelloBoard {
    const board = new OthelloBoard([], "B");
    board.reset();
    return board;
  }

  public static fromString(boardString: string, currentPlayer: Player = "B"): OthelloBoard | InvalidBoardResult {
    const rows = boardString.split("\n");

    if (rows.length !== this.BOARD_SIZE) {
      return { error: "Board must contain exactly 8 rows." };
    }

    for (const row of rows) {
      if (row.length !== this.BOARD_SIZE) {
        return { error: "Each row must contain exactly 8 fields." };
      }

      if (!/^[BW.]+$/.test(row)) {
        return { error: "Board can only contain the characters B, W, or ." };
      }
    }

    const board = rows.map((row) => row.split(""));
    return new OthelloBoard(board, currentPlayer);
  }

  public reset(): void {
    const board = [
      [".", ".", ".", ".", ".", ".", ".", "."],
      [".", ".", ".", ".", ".", ".", ".", "."],
      [".", ".", ".", ".", ".", ".", ".", "."],
      [".", ".", ".", "W", "B", ".", ".", "."],
      [".", ".", ".", "B", "W", ".", ".", "."],
      [".", ".", ".", ".", ".", ".", ".", "."],
      [".", ".", ".", ".", ".", ".", ".", "."],
      [".", ".", ".", ".", ".", ".", ".", "."],
    ];
    this.board = board;
    this.currentPlayer = "B";
  }

  /**
   * Returns the current player whose turn it is.
   */
  public getCurrentPlayer(): Player {
    return this.currentPlayer;
  }

  /**
   * Checks if a move at the given position is valid for the specified player.
   * Returns the flipped positions if valid, or null if invalid.
   */
  private getMoveResult(position: Position, player: Player): Position[] | null {
    // Check if position is on board
    if (!OthelloBoard.isOnBoard(position.row, position.col)) {
      return null;
    }

    // Check if position is empty
    const currentCell = this.board[position.row]?.[position.col];
    if (currentCell !== ".") {
      return null;
    }

    const enemy: Player = player === "B" ? "W" : "B";
    const directions: Array<[number, number]> = [
      [-1, -1],
      [-1, 0],
      [-1, 1],
      [0, -1],
      [0, 1],
      [1, -1],
      [1, 0],
      [1, 1],
    ];

    const flippedPositions: Position[] = [];

    for (const [deltaRow, deltaCol] of directions) {
      let r = position.row + deltaRow;
      let c = position.col + deltaCol;
      const path: Position[] = [];

      while (OthelloBoard.isOnBoard(r, c) && this.board[r]?.[c] === enemy) {
        path.push({ row: r, col: c });
        r += deltaRow;
        c += deltaCol;
      }

      if (path.length > 0 && OthelloBoard.isOnBoard(r, c) && this.board[r]?.[c] === player) {
        flippedPositions.push(...path);
      }
    }

    return flippedPositions.length > 0 ? flippedPositions : null;
  }

  /**
   * Returns all valid moves for the current player.
   */
  public getValidMoves(): ValidMovesResult {
    const moves: Move[] = [];

    for (let row = 0; row < OthelloBoard.BOARD_SIZE; row += 1) {
      for (let col = 0; col < OthelloBoard.BOARD_SIZE; col += 1) {
        const flippedPositions = this.getMoveResult({ row, col }, this.currentPlayer);

        if (flippedPositions !== null) {
          moves.push({
            position: { row, col },
            flippedPositions,
          });
        }
      }
    }

    return { moves };
  }

  /**
   * Returns the current game statistics, including the count of black and white stones on the board.
   */
  public getGameStatistics(): GameStatistics {
    let black = 0;
    let white = 0;

    for (let row = 0; row < OthelloBoard.BOARD_SIZE; row += 1) {
      for (let col = 0; col < OthelloBoard.BOARD_SIZE; col += 1) {
        const cell = this.board[row]?.[col];
        if (cell === "B") {
          black += 1;
        } else if (cell === "W") {
          white += 1;
        }
      }
    }

    return { black, white };
  }

  /**
   * Attempts to apply a move for the current player at the given position.
   * If successful, applies the move, flips opponent pieces, switches to the next player, and returns true.
   * If the move is invalid, returns false without modifying the board.
   */
  public tryApplyMove(position: Position): boolean;
  public tryApplyMove(position: string): boolean;
  public tryApplyMove(position: Position | string): boolean {
    // Parse string position if needed
    const pos = typeof position === "string" ? OthelloBoard.parsePosition(position) : position;

    if (pos === null) {
      return false;
    }

    // Verify the move is valid for the current player
    const flippedPositions = this.getMoveResult(pos, this.currentPlayer);
    if (flippedPositions === null) {
      return false;
    }

    // Place the player's piece at the move position
    const moveRow = this.board[pos.row];
    if (moveRow) {
      moveRow[pos.col] = this.currentPlayer;
    }

    // Flip all opponent pieces (use the validated flipped positions)
    for (const flippedPos of flippedPositions) {
      const row = this.board[flippedPos.row];
      if (row) {
        row[flippedPos.col] = this.currentPlayer;
      }
    }

    // Switch to the other player
    this.currentPlayer = this.currentPlayer === "B" ? "W" : "B";

    return true;
  }

  public toString(): string {
    return this.board.map((row) => row.join("")).join("\n");
  }

  /**
   * Returns a formatted string representation of the board with row/column labels
   * and visual disc representations.
   */
  public toFormattedString(): string {
    const lines: string[] = [];

    lines.push("   A B C D E F G H");
    lines.push("  ┌───────────────┐");

    for (let row = 0; row < OthelloBoard.BOARD_SIZE; row++) {
      const rowNum = row + 1;
      const cells =
        this.board[row]
          ?.map((cell) => {
            if (cell === "B") return "●"; // Black disc
            if (cell === "W") return "○"; // White disc
            return " "; // Empty
          })
          .join(" ") || "";
      lines.push(`${rowNum} │${cells}│`);
    }

    lines.push("  └───────────────┘");

    return lines.join("\n");
  }

  private static isOnBoard(row: number, col: number): boolean {
    return row >= 0 && row < OthelloBoard.BOARD_SIZE && col >= 0 && col < OthelloBoard.BOARD_SIZE;
  }

  /**
   * Parses a string position like "A1" into a Position object.
   * Column: A-H (case insensitive) maps to 0-7
   * Row: 1-8 maps to 0-7
   * Returns null if the format is invalid.
   */
  private static parsePosition(position: string): Position | null {
    if (position.length < 2 || position.length > 3) {
      return null;
    }

    const colChar = position[0]?.toUpperCase();
    const rowStr = position.slice(1);

    // Parse column (A-H)
    if (!colChar || colChar < "A" || colChar > "H") {
      return null;
    }
    const col = colChar.charCodeAt(0) - "A".charCodeAt(0);

    // Parse row (1-8)
    const row = parseInt(rowStr, 10);
    if (isNaN(row) || row < 1 || row > 8) {
      return null;
    }

    return { row: row - 1, col };
  }
}

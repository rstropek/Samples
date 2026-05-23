import { describe, expect, test } from "vitest";
import { OthelloBoard, isInvalidBoardResult } from "./index.js";

describe("OthelloBoard", () => {
  test("creates a board with standard initial setup", () => {
    const board = OthelloBoard.createEmpty();
    expect(board).toBeInstanceOf(OthelloBoard);
    expect(board.getCurrentPlayer()).toBe("B");

    const resultBlack = board.getValidMoves();
    expect(resultBlack.moves.length).toBe(4);
    const positionsBlack = resultBlack.moves.map((m) => m.position);
    expect(positionsBlack).toContainEqual({ row: 2, col: 3 });
    expect(positionsBlack).toContainEqual({ row: 3, col: 2 });
    expect(positionsBlack).toContainEqual({ row: 4, col: 5 });
    expect(positionsBlack).toContainEqual({ row: 5, col: 4 });

    // Create separate board for white to test white's valid moves
    const boardWhite = OthelloBoard.fromString(
      "........\n........\n........\n...WB...\n...BW...\n........\n........\n........",
      "W",
    );
    if (!isInvalidBoardResult(boardWhite)) {
      const resultWhite = boardWhite.getValidMoves();
      expect(resultWhite.moves.length).toBe(4);
      const positionsWhite = resultWhite.moves.map((m) => m.position);
      expect(positionsWhite).toContainEqual({ row: 2, col: 4 });
      expect(positionsWhite).toContainEqual({ row: 3, col: 5 });
      expect(positionsWhite).toContainEqual({ row: 4, col: 2 });
      expect(positionsWhite).toContainEqual({ row: 5, col: 3 });
    }
  });

  describe("fromString", () => {
    test("creates a valid board from a valid string", () => {
      const boardString = "........\n........\n........\n...WB...\n...BW...\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(false);
      expect(result).toBeInstanceOf(OthelloBoard);
    });

    test("rejects board with too few rows", () => {
      const boardString = "........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(true);
      if (isInvalidBoardResult(result)) {
        expect(result.error).toBe("Board must contain exactly 8 rows.");
      }
    });

    test("rejects board with too many rows", () => {
      const boardString =
        "........\n" +
        "........\n" +
        "........\n" +
        "...WB...\n" +
        "...BW...\n" +
        "........\n" +
        "........\n" +
        "........\n" +
        "........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(true);
      if (isInvalidBoardResult(result)) {
        expect(result.error).toBe("Board must contain exactly 8 rows.");
      }
    });

    test("rejects board with row too short", () => {
      const boardString = "........\n.......\n........\n...WB...\n...BW...\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(true);
      if (isInvalidBoardResult(result)) {
        expect(result.error).toBe("Each row must contain exactly 8 fields.");
      }
    });

    test("rejects board with row too long", () => {
      const boardString = "........\n.........\n........\n...WB...\n...BW...\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(true);
      if (isInvalidBoardResult(result)) {
        expect(result.error).toBe("Each row must contain exactly 8 fields.");
      }
    });

    test("rejects board with invalid characters", () => {
      const boardString = "........\n........\n........\n...WB...\n...BX...\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString);
      expect(isInvalidBoardResult(result)).toBe(true);
      if (isInvalidBoardResult(result)) {
        expect(result.error).toBe("Board can only contain the characters B, W, or .");
      }
    });
  });

  describe("getValidMoves", () => {
    test("returns empty moves array when no valid moves exist", () => {
      const boardString = "........\n........\n........\n...BBB..\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        expect(moves.moves.length).toBe(0);
      }
    });

    test("returns correct flipped positions for a simple horizontal capture", () => {
      const boardString = "........\n........\n........\n..BW....\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 3 && m.position.col === 4);
        expect(move).toBeDefined();
        if (move) {
          expect(move.flippedPositions).toContainEqual({ row: 3, col: 3 });
          expect(move.flippedPositions.length).toBe(1);
        }
      }
    });

    test("returns correct flipped positions for a simple vertical capture", () => {
      const boardString = "........\n...B....\n...W....\n........\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 3 && m.position.col === 3);
        expect(move).toBeDefined();
        if (move) {
          expect(move.flippedPositions).toContainEqual({ row: 2, col: 3 });
          expect(move.flippedPositions.length).toBe(1);
        }
      }
    });

    test("returns correct flipped positions for a diagonal capture", () => {
      const boardString = "........\n...B....\n....W...\n........\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 3 && m.position.col === 5);
        expect(move).toBeDefined();
        if (move) {
          expect(move.flippedPositions).toContainEqual({ row: 2, col: 4 });
          expect(move.flippedPositions.length).toBe(1);
        }
      }
    });

    test("returns multiple flipped positions when capturing in multiple directions", () => {
      const boardString = ".....B..\n..BWW...\n...BWW..\n...B.W..\n.....B..\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 1 && m.position.col === 5);
        expect(move).toBeDefined();
        if (move) {
          // Should flip pieces in both horizontal and vertical directions
          expect(move.flippedPositions.length).toBeGreaterThan(1);
          expect(move.flippedPositions).toContainEqual({ row: 1, col: 3 });
          expect(move.flippedPositions).toContainEqual({ row: 1, col: 4 });
          expect(move.flippedPositions).toContainEqual({ row: 2, col: 5 });
          expect(move.flippedPositions).toContainEqual({ row: 3, col: 5 });
          expect(move.flippedPositions).toContainEqual({ row: 2, col: 4 });
        }
      }
    });

    test("does not allow move on occupied cell", () => {
      const board = OthelloBoard.createEmpty();
      const moves = board.getValidMoves();

      // Check that none of the valid moves are on the initial occupied positions
      const positions = moves.moves.map((m) => m.position);
      expect(positions).not.toContainEqual({ row: 3, col: 3 }); // W
      expect(positions).not.toContainEqual({ row: 3, col: 4 }); // B
      expect(positions).not.toContainEqual({ row: 4, col: 3 }); // B
      expect(positions).not.toContainEqual({ row: 4, col: 4 }); // W
    });

    test("captures multiple pieces in a line", () => {
      const boardString = "........\n........\n........\nBWWW....\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 3 && m.position.col === 4);
        expect(move).toBeDefined();
        if (move) {
          expect(move.flippedPositions.length).toBe(3);
          expect(move.flippedPositions).toContainEqual({ row: 3, col: 1 });
          expect(move.flippedPositions).toContainEqual({ row: 3, col: 2 });
          expect(move.flippedPositions).toContainEqual({ row: 3, col: 3 });
        }
      }
    });

    test("does not capture beyond board boundaries", () => {
      const boardString = "WWB.....\n........\n........\n........\n........\n........\n........\n........";

      const result = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(result)) {
        const moves = result.getValidMoves();
        // Should not find any valid moves from these white pieces
        // because there's no black piece to sandwich them
        expect(moves.moves.length).toBe(0);
      }
    });
  });

  describe("move validation through tryApplyMove", () => {
    test("rejects occupied position", () => {
      const board = OthelloBoard.createEmpty();
      const result = board.tryApplyMove({ row: 3, col: 3 });
      expect(result).toBe(false);
    });

    test("rejects position with no captures", () => {
      const board = OthelloBoard.createEmpty();
      const result = board.tryApplyMove({ row: 0, col: 0 });
      expect(result).toBe(false);
    });

    test("rejects out of bounds position", () => {
      const board = OthelloBoard.createEmpty();
      const result = board.tryApplyMove({ row: -1, col: 0 });
      expect(result).toBe(false);
    });

    test("accepts and applies valid move with flips", () => {
      const boardString = "........\n........\n........\n..BW....\n........\n........\n........\n........";
      const result = OthelloBoard.fromString(boardString, "B");

      if (!isInvalidBoardResult(result)) {
        const success = result.tryApplyMove({ row: 3, col: 4 });
        expect(success).toBe(true);
        // Verify the flip occurred
        const boardStr = result.toString();
        const rows = boardStr.split("\n");
        expect(rows[3]?.[3]).toBe("B"); // Flipped piece
        expect(rows[3]?.[4]).toBe("B"); // Placed piece
      }
    });

    test("applies multi-directional capture correctly", () => {
      const boardString = ".....B..\n..BWW...\n...BWW..\n...B.W..\n.....B..\n........\n........\n........";
      const result = OthelloBoard.fromString(boardString, "B");

      if (!isInvalidBoardResult(result)) {
        const success = result.tryApplyMove({ row: 1, col: 5 });
        expect(success).toBe(true);
        const boardStr = result.toString();
        const rows = boardStr.split("\n");
        // Check that pieces were flipped
        expect(rows[1]?.[3]).toBe("B");
        expect(rows[1]?.[4]).toBe("B");
        expect(rows[2]?.[5]).toBe("B");
        expect(rows[3]?.[5]).toBe("B");
        expect(rows[2]?.[4]).toBe("B");
      }
    });

    test("works for both black and white players via currentPlayer", () => {
      const boardString = "........\n........\n........\n..BW....\n........\n........\n........\n........";

      // Test black player
      const boardBlack = OthelloBoard.fromString(boardString, "B");
      if (!isInvalidBoardResult(boardBlack)) {
        expect(boardBlack.getCurrentPlayer()).toBe("B");
        const blackResult = boardBlack.tryApplyMove({ row: 3, col: 4 });
        expect(blackResult).toBe(true);
        expect(boardBlack.getCurrentPlayer()).toBe("W"); // Player switched
      }

      // Test white player
      const boardWhite = OthelloBoard.fromString(boardString, "W");
      if (!isInvalidBoardResult(boardWhite)) {
        expect(boardWhite.getCurrentPlayer()).toBe("W");
        const whiteResult = boardWhite.tryApplyMove({ row: 3, col: 1 });
        expect(whiteResult).toBe(true);
        expect(boardWhite.getCurrentPlayer()).toBe("B"); // Player switched
      }
    });
  });

  describe("isInvalidBoardResult", () => {
    test("returns true for invalid board result", () => {
      const invalidResult = { error: "Some error message" };
      expect(isInvalidBoardResult(invalidResult)).toBe(true);
    });

    test("returns false for valid board instance", () => {
      const board = OthelloBoard.createEmpty();
      expect(isInvalidBoardResult(board)).toBe(false);
    });

    test("returns false for null", () => {
      expect(isInvalidBoardResult(null)).toBe(false);
    });

    test("returns false for undefined", () => {
      expect(isInvalidBoardResult(undefined)).toBe(false);
    });

    test("returns false for object without error property", () => {
      const obj = { foo: "bar" };
      expect(isInvalidBoardResult(obj)).toBe(false);
    });

    test("returns false for object with non-string error property", () => {
      const obj = { error: 123 };
      expect(isInvalidBoardResult(obj)).toBe(false);
    });
  });

  describe("tryApplyMove", () => {
    test("returns true and applies valid move (both Position object and string)", () => {
      // Test with Position object
      const board1 = OthelloBoard.createEmpty();
      expect(board1.getCurrentPlayer()).toBe("B");
      const moves1 = board1.getValidMoves();
      const move = moves1.moves[0];

      expect(move).toBeDefined();
      if (!move) return;

      const result1 = board1.tryApplyMove(move.position);
      expect(result1).toBe(true);
      expect(board1.getCurrentPlayer()).toBe("W"); // Player switched
      const boardString1 = board1.toString();
      const rows1 = boardString1.split("\n");
      const row1 = rows1[move.position.row];
      expect(row1?.[move.position.col]).toBe("B");

      // Test with string position
      const board2 = OthelloBoard.createEmpty();
      expect(board2.getCurrentPlayer()).toBe("B");
      const moves2 = board2.getValidMoves();
      const moveD3 = moves2.moves.find((m) => m.position.row === 2 && m.position.col === 3);
      expect(moveD3).toBeDefined();

      const result2 = board2.tryApplyMove("D3");
      expect(result2).toBe(true);
      expect(board2.getCurrentPlayer()).toBe("W"); // Player switched
      const boardString2 = board2.toString();
      const rows2 = boardString2.split("\n");
      expect(rows2[2]?.[3]).toBe("B");
    });

    test("returns false and does not modify board for invalid move (both Position object and string)", () => {
      const board = OthelloBoard.createEmpty();
      expect(board.getCurrentPlayer()).toBe("B");
      const originalString = board.toString();

      // Test with Position object
      const invalidMove = { row: 0, col: 0 };
      const result1 = board.tryApplyMove(invalidMove);
      expect(result1).toBe(false);
      expect(board.toString()).toBe(originalString);
      expect(board.getCurrentPlayer()).toBe("B"); // Player unchanged

      // Test with string position (board should still be unchanged)
      const result2 = board.tryApplyMove("A1");
      expect(result2).toBe(false);
      expect(board.toString()).toBe(originalString);
      expect(board.getCurrentPlayer()).toBe("B"); // Player unchanged
    });

    test("returns false for occupied position (both Position object and string)", () => {
      const board = OthelloBoard.createEmpty();
      expect(board.getCurrentPlayer()).toBe("B");
      const originalString = board.toString();

      // Test with Position object
      const invalidMove = {
        row: 3,
        col: 3, // Already occupied by W
      };
      const result1 = board.tryApplyMove(invalidMove);
      expect(result1).toBe(false);
      expect(board.toString()).toBe(originalString);
      expect(board.getCurrentPlayer()).toBe("B"); // Player unchanged

      // Test with string position - D4 is (3, 3) (board should still be unchanged)
      const result2 = board.tryApplyMove("D4");
      expect(result2).toBe(false);
      expect(board.toString()).toBe(originalString);
      expect(board.getCurrentPlayer()).toBe("B"); // Player unchanged
    });

    test("modifies board in-place when successful (both Position object and string)", () => {
      // Test with Position object
      const board1 = OthelloBoard.createEmpty();
      const originalString1 = board1.toString();
      const moves1 = board1.getValidMoves();
      const move = moves1.moves[0];

      expect(move).toBeDefined();
      if (!move) return;

      const result1 = board1.tryApplyMove(move.position);
      expect(result1).toBe(true);
      expect(board1.toString()).not.toBe(originalString1);

      // Test with string position
      const board2 = OthelloBoard.createEmpty();
      const originalString2 = board2.toString();

      const result2 = board2.tryApplyMove("D3");
      expect(result2).toBe(true);
      expect(board2.toString()).not.toBe(originalString2);
    });

    test("flips opponent pieces when successful (both Position object and string)", () => {
      // Test with Position object
      const boardString1 = "........\n........\n........\n..BW....\n........\n........\n........\n........";
      const result1 = OthelloBoard.fromString(boardString1, "B");

      if (!isInvalidBoardResult(result1)) {
        const moves = result1.getValidMoves();
        const move = moves.moves.find((m) => m.position.row === 3 && m.position.col === 4);

        if (move) {
          const success = result1.tryApplyMove(move.position);

          expect(success).toBe(true);
          const newBoardString = result1.toString();
          const rows = newBoardString.split("\n");
          const row = rows[3];

          // Check that the piece at (3, 3) was flipped to B
          expect(row?.[3]).toBe("B");
          // Check that the new piece was placed
          expect(row?.[4]).toBe("B");
        }
      }

      // Test with string position - E4 is (3, 4)
      const boardString2 = "........\n........\n........\n..BW....\n........\n........\n........\n........";
      const result2 = OthelloBoard.fromString(boardString2, "B");

      if (!isInvalidBoardResult(result2)) {
        const success = result2.tryApplyMove("E4");

        expect(success).toBe(true);
        const newBoardString = result2.toString();
        const rows = newBoardString.split("\n");
        const row = rows[3];

        // Check that the piece at (3, 3) was flipped to B
        expect(row?.[3]).toBe("B");
        // Check that the new piece was placed at (3, 4)
        expect(row?.[4]).toBe("B");
      }
    });

    test("accepts lowercase string position", () => {
      const board = OthelloBoard.createEmpty();
      const moves = board.getValidMoves();

      // Find a valid move at position (2, 3) which is D3
      const move = moves.moves.find((m) => m.position.row === 2 && m.position.col === 3);
      expect(move).toBeDefined();

      const result = board.tryApplyMove("d3");
      expect(result).toBe(true);
    });

    test("returns false for invalid string position format", () => {
      const board = OthelloBoard.createEmpty();
      const originalString = board.toString();

      const result = board.tryApplyMove("XYZ");
      expect(result).toBe(false);
      expect(board.toString()).toBe(originalString);
    });

    test("returns false for out of bounds positions (string)", () => {
      const board = OthelloBoard.createEmpty();
      const originalString = board.toString();

      // Column out of bounds
      const result1 = board.tryApplyMove("I1");
      expect(result1).toBe(false);
      expect(board.toString()).toBe(originalString);

      // Row out of bounds (board should still be unchanged)
      const result2 = board.tryApplyMove("A9");
      expect(result2).toBe(false);
      expect(board.toString()).toBe(originalString);

      // Row 0 (invalid, board should still be unchanged)
      const result3 = board.tryApplyMove("A0");
      expect(result3).toBe(false);
      expect(board.toString()).toBe(originalString);
    });

    test("correctly maps corner and edge positions (string)", () => {
      // Test A1 (row 0, col 0)
      const boardString1 = ".W......\n........\n........\n........\n........\n........\n........\n........";
      const result1 = OthelloBoard.fromString(boardString1, "B");

      if (!isInvalidBoardResult(result1)) {
        const moves = result1.getValidMoves();
        const hasA1Move = moves.moves.some((m) => m.position.row === 0 && m.position.col === 0);
        const moveResult = result1.tryApplyMove("A1");
        expect(moveResult).toBe(hasA1Move);
      }

      // Test H8 (row 7, col 7)
      const boardString2 = "........\n........\n........\n........\n........\n........\n.......W\n........";
      const result2 = OthelloBoard.fromString(boardString2, "B");

      if (!isInvalidBoardResult(result2)) {
        const moves = result2.getValidMoves();
        const hasH8Move = moves.moves.some((m) => m.position.row === 7 && m.position.col === 7);
        const moveResult = result2.tryApplyMove("H8");
        expect(moveResult).toBe(hasH8Move);
      }

      // Test C4 (row 3, col 2) - middle position
      const board3 = OthelloBoard.createEmpty();
      const moves3 = board3.getValidMoves();
      const moveC4 = moves3.moves.find((m) => m.position.row === 3 && m.position.col === 2);
      expect(moveC4).toBeDefined();

      const result3 = board3.tryApplyMove("C4");
      expect(result3).toBe(true);
    });
  });

  describe("getGameStatistics", () => {
    test("returns correct stone counts for initial board", () => {
      const board = OthelloBoard.createEmpty();
      const stats = board.getGameStatistics();

      expect(stats.black).toBe(2);
      expect(stats.white).toBe(2);
    });

    test("returns correct stone counts after moves", () => {
      const boardString = "........\n........\n........\n..BBB...\n........\n........\n........\n........";
      const result = OthelloBoard.fromString(boardString);

      if (!isInvalidBoardResult(result)) {
        const stats = result.getGameStatistics();
        expect(stats.black).toBe(3);
        expect(stats.white).toBe(0);
      }
    });

    test("returns zero for both when board is empty", () => {
      const boardString = "........\n........\n........\n........\n........\n........\n........\n........";
      const result = OthelloBoard.fromString(boardString);

      if (!isInvalidBoardResult(result)) {
        const stats = result.getGameStatistics();
        expect(stats.black).toBe(0);
        expect(stats.white).toBe(0);
      }
    });
  });

  describe("toFormattedString", () => {
    test("returns formatted board with labels and borders", () => {
      const board = OthelloBoard.createEmpty();
      const formatted = board.toFormattedString();

      expect(formatted).toContain("   A B C D E F G H");
      expect(formatted).toContain("┌───────────────┐");
      expect(formatted).toContain("└───────────────┘");
      expect(formatted).toContain("1 │");
      expect(formatted).toContain("8 │");
    });

    test("displays black and white discs with correct symbols", () => {
      const board = OthelloBoard.createEmpty();
      const formatted = board.toFormattedString();

      // Black discs should be displayed as ●
      expect(formatted).toContain("●");
      // White discs should be displayed as ○
      expect(formatted).toContain("○");
    });

    test("displays empty cells as spaces", () => {
      const boardString = "........\n........\n........\n........\n........\n........\n........\n........";
      const result = OthelloBoard.fromString(boardString);

      if (!isInvalidBoardResult(result)) {
        const formatted = result.toFormattedString();
        const lines = formatted.split("\n");

        // Check that row 1 contains only spaces between borders
        expect(lines[2]).toMatch(/1 │\s{15}│/);
      }
    });

    test("formats board correctly with complex setup", () => {
      const boardString = "BBBBBBBB\nWWWWWWWW\nBBBBBBBB\nWWWWWWWW\nBBBBBBBB\nWWWWWWWW\nBBBBBBBB\nWWWWWWWW";
      const result = OthelloBoard.fromString(boardString);

      if (!isInvalidBoardResult(result)) {
        const formatted = result.toFormattedString();
        const lines = formatted.split("\n");

        // Should have 11 lines (header + top border + 8 rows + bottom border)
        expect(lines.length).toBe(11);

        // Each row should have alternating patterns
        expect(lines[2]).toContain("● ● ● ● ● ● ● ●"); // Row 1
        expect(lines[3]).toContain("○ ○ ○ ○ ○ ○ ○ ○"); // Row 2
      }
    });
  });
});

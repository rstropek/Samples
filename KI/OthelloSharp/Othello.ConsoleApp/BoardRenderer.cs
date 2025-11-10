using Othello.GameLogic;

namespace Othello.ConsoleApp;

/// <summary>
/// Renders the Othello board to the console.
/// </summary>
public class BoardRenderer
{
    /// <summary>
    /// Renders the board to the console with coordinates and disc counts.
    /// </summary>
    /// <param name="gameState">The current game state to render.</param>
    /// <param name="showValidMoves">If true, shows valid moves with asterisks.</param>
    public void Render(GameState gameState, bool showValidMoves = false)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║         OTHELLO / REVERSI          ║");
        Console.WriteLine("╚════════════════════════════════════╝");
        Console.WriteLine();

        var validMoves = showValidMoves ? gameState.GetValidMoves() : new List<Position>();

        // Column headers
        Console.Write("   ");
        for (int col = 0; col < 8; col++)
        {
            Console.Write($" {col} ");
        }
        Console.WriteLine();

        // Top border
        Console.WriteLine("  ┌" + string.Join("┬", Enumerable.Repeat("───", 8)) + "┐");

        // Board rows
        for (int row = 0; row < 8; row++)
        {
            Console.Write($"{row} │");

            for (int col = 0; col < 8; col++)
            {
                var position = new Position(row, col);
                var disc = gameState.Board.GetDisc(position);

                if (disc == Player.Black)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" ● ");
                    Console.ResetColor();
                }
                else if (disc == Player.White)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" ○ ");
                    Console.ResetColor();
                }
                else if (validMoves.Contains(position))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" * ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("   ");
                }

                Console.Write("│");
            }

            Console.WriteLine();

            // Row separator
            if (row < 7)
            {
                Console.WriteLine("  ├" + string.Join("┼", Enumerable.Repeat("───", 8)) + "┤");
            }
        }

        // Bottom border
        Console.WriteLine("  └" + string.Join("┴", Enumerable.Repeat("───", 8)) + "┘");

        Console.WriteLine();

        // Score
        var blackCount = gameState.GetDiscCount(Player.Black);
        var whiteCount = gameState.GetDiscCount(Player.White);

        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"● Black: {blackCount}");
        Console.ResetColor();
        Console.Write("   ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"○ White: {whiteCount}");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine();

        // Current player
        if (!gameState.IsGameOver)
        {
            Console.ForegroundColor = gameState.CurrentPlayer == Player.Black ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.WriteLine($"  Current player: {gameState.CurrentPlayer}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  GAME OVER - {gameState.GetGameResult()}");
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Displays a message to the console.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="color">The color to use for the message.</param>
    public void DisplayMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
    }
}


using Othello.GameLogic;

namespace Othello.ConsoleApp;

/// <summary>
/// Handles user input for the Othello game.
/// </summary>
public class InputHandler
{
    /// <summary>
    /// Gets a move from the user.
    /// </summary>
    /// <returns>The position entered by the user, or null if the user wants to quit.</returns>
    public Position? GetMove()
    {
        while (true)
        {
            Console.Write("  Enter your move (row col), 'h' for help, or 'q' to quit: ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid input. Please try again.");
                Console.ResetColor();
                continue;
            }

            if (input == "q" || input == "quit")
            {
                return null;
            }

            if (input == "h" || input == "help")
            {
                DisplayHelp();
                continue;
            }

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid format. Please enter row and column separated by a space.");
                Console.ResetColor();
                continue;
            }

            if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid input. Row and column must be numbers.");
                Console.ResetColor();
                continue;
            }

            if (!Position.IsValid(row, col))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid position. Row and column must be between 0 and 7.");
                Console.ResetColor();
                continue;
            }

            return new Position(row, col);
        }
    }

    /// <summary>
    /// Asks the user if they want to play again.
    /// </summary>
    /// <returns>True if the user wants to play again; otherwise, false.</returns>
    public bool AskPlayAgain()
    {
        while (true)
        {
            Console.Write("  Would you like to play again? (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "y" || input == "yes")
                return true;
            if (input == "n" || input == "no")
                return false;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Please enter 'y' for yes or 'n' for no.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Displays help information.
    /// </summary>
    private void DisplayHelp()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                     OTHELLO RULES                         ║");
        Console.WriteLine("  ╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("  • Black (●) always goes first");
        Console.WriteLine("  • Place a disc to sandwich opponent's discs");
        Console.WriteLine("  • Sandwiched discs flip to your color");
        Console.WriteLine("  • Valid moves are shown with asterisks (*)");
        Console.WriteLine("  • If you have no valid moves, your turn is skipped");
        Console.WriteLine("  • Game ends when neither player can move");
        Console.WriteLine("  • Player with the most discs wins!");
        Console.WriteLine();
        Console.WriteLine("  ╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                     HOW TO PLAY                           ║");
        Console.WriteLine("  ╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("  • Enter coordinates as: row col (e.g., '2 3')");
        Console.WriteLine("  • Rows and columns are numbered 0-7");
        Console.WriteLine("  • Type 'h' or 'help' to see this message again");
        Console.WriteLine("  • Type 'q' or 'quit' to exit the game");
        Console.WriteLine();
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey(true);
    }
}


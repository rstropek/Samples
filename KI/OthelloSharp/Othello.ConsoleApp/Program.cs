using Othello.ConsoleApp;
using Othello.GameLogic;

// Welcome message
Console.Clear();
Console.WriteLine("╔════════════════════════════════════════════════════╗");
Console.WriteLine("║                                                    ║");
Console.WriteLine("║          Welcome to OTHELLO / REVERSI!             ║");
Console.WriteLine("║                                                    ║");
Console.WriteLine("╚════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("  Press any key to start...");
Console.ReadKey(true);

var renderer = new BoardRenderer();
var inputHandler = new InputHandler();
bool playAgain = true;

while (playAgain)
{
    // Initialize the game
    var game = new GameState();
    game.Initialize();

    // Main game loop
    while (!game.IsGameOver)
    {
        // Render the board with valid moves highlighted
        renderer.Render(game, showValidMoves: true);

        // Check if current player has valid moves
        if (!game.CurrentPlayerHasValidMoves())
        {
            renderer.DisplayMessage($"{game.CurrentPlayer} has no valid moves. Turn passes to opponent.", ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey(true);
            
            // The game will handle switching players internally
            // If neither player has moves, IsGameOver will be true
            continue;
        }

        // Get valid moves
        var validMoves = game.GetValidMoves();
        renderer.DisplayMessage($"Valid moves: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Column})"))}", ConsoleColor.Cyan);
        Console.WriteLine();

        // Get move from user
        var position = inputHandler.GetMove();

        // Check if user wants to quit
        if (position == null)
        {
            Console.WriteLine();
            renderer.DisplayMessage("Thanks for playing!", ConsoleColor.Green);
            return;
        }

        // Attempt to make the move
        if (game.MakeMove(position.Value))
        {
            renderer.DisplayMessage($"Move successful at ({position.Value.Row}, {position.Value.Column})!", ConsoleColor.Green);
            Thread.Sleep(500); // Brief pause to show the message
        }
        else
        {
            renderer.DisplayMessage("Invalid move! Please try again.", ConsoleColor.Red);
            Console.WriteLine();
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey(true);
        }
    }

    // Game over - display final board and results
    renderer.Render(game, showValidMoves: false);
    Console.WriteLine();

    var blackCount = game.GetDiscCount(Player.Black);
    var whiteCount = game.GetDiscCount(Player.White);
    
    Console.WriteLine("  ╔════════════════════════════════════╗");
    Console.WriteLine("  ║          FINAL RESULTS             ║");
    Console.WriteLine("  ╚════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine($"  Black (●): {blackCount} discs");
    Console.WriteLine($"  White (○): {whiteCount} discs");
    Console.WriteLine();

    var winner = game.GetWinner();
    if (winner == null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  It's a DRAW!");
    }
    else
    {
        Console.ForegroundColor = winner == Player.Black ? ConsoleColor.White : ConsoleColor.DarkGray;
        Console.WriteLine($"  {winner} WINS!");
    }
    Console.ResetColor();
    Console.WriteLine();

    // Ask if player wants to play again
    playAgain = inputHandler.AskPlayAgain();
}

Console.WriteLine();
renderer.DisplayMessage("Thanks for playing Othello!", ConsoleColor.Green);

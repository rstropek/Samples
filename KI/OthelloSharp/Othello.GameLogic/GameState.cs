namespace Othello.GameLogic;

/// <summary>
/// Manages the state of an Othello game including the board, current player, and game status.
/// </summary>
public class GameState
{
    private readonly MoveValidator validator = new();

    /// <summary>
    /// Gets the game board.
    /// </summary>
    public Board Board { get; } = new();

    /// <summary>
    /// Gets the current player whose turn it is.
    /// </summary>
    public Player CurrentPlayer { get; private set; } = Player.Black; // Black always goes first

    /// <summary>
    /// Gets a value indicating whether the game is over.
    /// </summary>
    public bool IsGameOver => !validator.HasValidMoves(Board, Player.Black) && 
                               !validator.HasValidMoves(Board, Player.White);

    /// <summary>
    /// Initializes a new game with the standard starting position.
    /// </summary>
    public void Initialize()
    {
        Board.Initialize();
        CurrentPlayer = Player.Black;
    }

    /// <summary>
    /// Gets all valid moves for the current player.
    /// </summary>
    /// <returns>A list of valid positions where the current player can move.</returns>
    public List<Position> GetValidMoves() => validator.GetValidMoves(Board, CurrentPlayer);

    /// <summary>
    /// Gets all valid moves for a specific player.
    /// </summary>
    /// <param name="player">The player to get valid moves for.</param>
    /// <returns>A list of valid positions where the player can move.</returns>
    public List<Position> GetValidMoves(Player player) => validator.GetValidMoves(Board, player);

    /// <summary>
    /// Gets the disc count for a specific player.
    /// </summary>
    /// <param name="player">The player whose discs to count.</param>
    /// <returns>The number of discs belonging to the player.</returns>
    public int GetDiscCount(Player player) => Board.CountDiscs(player);

    /// <summary>
    /// Makes a move for the current player at the specified position.
    /// </summary>
    /// <param name="position">The position where to place the disc.</param>
    /// <returns>True if the move was successful; false if invalid.</returns>
    public bool MakeMove(Position position)
    {
        if (!Board.PlaceDisc(position, CurrentPlayer, validator))
            return false;

        // Switch to next player, but only if they have valid moves
        var nextPlayer = CurrentPlayer.Opponent();
        if (validator.HasValidMoves(Board, nextPlayer))
        {
            CurrentPlayer = nextPlayer;
        }
        // If opponent has no valid moves but current player does, current player keeps the turn
        else if (!validator.HasValidMoves(Board, CurrentPlayer))
        {
            // Game is over - neither player can move
            // Keep current player as-is
        }
        // Else: current player keeps the turn because opponent has no valid moves

        return true;
    }

    /// <summary>
    /// Checks if the current player has any valid moves.
    /// </summary>
    /// <returns>True if the current player has at least one valid move.</returns>
    public bool CurrentPlayerHasValidMoves() => validator.HasValidMoves(Board, CurrentPlayer);
    
    /// <summary>
    /// Gets the winner of the game.
    /// </summary>
    /// <returns>The winning player, or null if the game is a draw or not yet over.</returns>
    public Player? GetWinner()
    {
        if (!IsGameOver)
            return null;

        var blackCount = Board.CountDiscs(Player.Black);
        var whiteCount = Board.CountDiscs(Player.White);

        if (blackCount > whiteCount)
            return Player.Black;
        if (whiteCount > blackCount)
            return Player.White;
        
        return null; // Draw
    }

    /// <summary>
    /// Gets the game result as a string.
    /// </summary>
    /// <returns>A string describing the game result.</returns>
    public string GetGameResult()
    {
        if (!IsGameOver)
            return "Game in progress";

        var winner = GetWinner();
        return winner switch
        {
            Player.Black => "Black wins!",
            Player.White => "White wins!",
            _ => "Draw!"
        };
    }
}


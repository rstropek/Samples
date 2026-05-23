namespace Othello.GameLogic;

/// <summary>
/// Represents the Othello game board.
/// </summary>
public class Board
{
    private readonly Player?[,] grid = new Player?[8, 8];

    /// <summary>
    /// Initializes the board to the standard Othello starting position.
    /// </summary>
    public void Initialize()
    {
        // Clear the board
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                grid[row, col] = null;
            }
        }

        // Set up the standard starting position
        // Center 2x2 square:
        //   3,3: White  3,4: Black
        //   4,3: Black  4,4: White
        grid[3, 3] = Player.White;
        grid[3, 4] = Player.Black;
        grid[4, 3] = Player.Black;
        grid[4, 4] = Player.White;
    }

    /// <summary>
    /// Gets the disc at the specified position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>The player who owns the disc at this position, or null if empty.</returns>
    public Player? GetDisc(Position position)
    {
        return grid[position.Row, position.Column];
    }

    /// <summary>
    /// Sets a disc at the specified position.
    /// </summary>
    /// <param name="position">The position to place the disc.</param>
    /// <param name="player">The player whose disc to place.</param>
    public void SetDisc(Position position, Player player)
    {
        grid[position.Row, position.Column] = player;
    }

    /// <summary>
    /// Checks if the specified position is occupied.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position has a disc, false if empty.</returns>
    public bool IsOccupied(Position position)
    {
        return grid[position.Row, position.Column] != null;
    }

    /// <summary>
    /// Counts the number of discs for a specific player.
    /// </summary>
    /// <param name="player">The player whose discs to count.</param>
    /// <returns>The number of discs belonging to the player.</returns>
    public int CountDiscs(Player player)
    {
        int count = 0;
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (grid[row, col] == player)
                {
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    /// Creates a deep copy of the board.
    /// </summary>
    /// <returns>A new Board instance with the same state.</returns>
    public Board Clone()
    {
        var newBoard = new Board();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                newBoard.grid[row, col] = grid[row, col];
            }
        }
        return newBoard;
    }

    /// <summary>
    /// Places a disc at the specified position and flips opponent discs according to Othello rules.
    /// </summary>
    /// <param name="position">The position to place the disc.</param>
    /// <param name="player">The player making the move.</param>
    /// <param name="validator">The move validator to use for determining which discs to flip.</param>
    /// <returns>True if the move was successful; false if the move was invalid.</returns>
    public bool PlaceDisc(Position position, Player player, MoveValidator validator)
    {
        if (!validator.IsValidMove(this, position, player))
            return false;

        // Get all discs to flip
        var toFlip = validator.GetFlippableDiscs(this, position, player);

        // Place the new disc
        SetDisc(position, player);

        // Flip all opponent discs
        foreach (var flipPosition in toFlip)
        {
            SetDisc(flipPosition, player);
        }

        return true;
    }
}


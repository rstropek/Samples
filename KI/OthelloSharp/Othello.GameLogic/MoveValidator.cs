namespace Othello.GameLogic;

/// <summary>
/// Validates moves according to Othello rules.
/// </summary>
public class MoveValidator
{
    /// <summary>
    /// Checks if a move is valid according to Othello rules.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="position">The position where the player wants to place a disc.</param>
    /// <param name="player">The player making the move.</param>
    /// <returns>True if the move is valid; otherwise, false.</returns>
    public bool IsValidMove(Board board, Position position, Player player)
    {
        // Position must be empty
        if (board.IsOccupied(position))
            return false;
        
        // Must flip at least one opponent disc
        return GetFlippableDiscs(board, position, player).Any();
    }

    /// <summary>
    /// Gets all positions of opponent discs that would be flipped if a disc is placed at the given position.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="position">The position where the player wants to place a disc.</param>
    /// <param name="player">The player making the move.</param>
    /// <returns>A list of positions that would be flipped.</returns>
    public List<Position> GetFlippableDiscs(Board board, Position position, Player player)
    {
        var flippable = new List<Position>();
        var opponent = player.Opponent();

        // Check all 8 directions
        foreach (var direction in Direction.All)
        {
            var discsInDirection = new List<Position>();
            var current = direction.GetNext(position);

            // Walk in this direction
            while (current.HasValue)
            {
                var disc = board.GetDisc(current.Value);
                
                // If empty, this direction doesn't yield any flips
                if (disc == null)
                    break;
                
                // If opponent's disc, continue looking
                if (disc == opponent)
                {
                    discsInDirection.Add(current.Value);
                    current = direction.GetNext(current.Value);
                }
                // If our disc, we found a valid line
                else if (disc == player)
                {
                    // Only add if there were opponent discs in between
                    if (discsInDirection.Any())
                    {
                        flippable.AddRange(discsInDirection);
                    }
                    break;
                }
            }
        }

        return flippable;
    }

    /// <summary>
    /// Gets all valid moves for a player on the current board.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="player">The player to get valid moves for.</param>
    /// <returns>A list of valid positions where the player can move.</returns>
    public List<Position> GetValidMoves(Board board, Player player)
    {
        var validMoves = new List<Position>();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var position = new Position(row, col);
                if (IsValidMove(board, position, player))
                {
                    validMoves.Add(position);
                }
            }
        }

        return validMoves;
    }

    /// <summary>
    /// Checks if a player has any valid moves on the current board.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the player has at least one valid move; otherwise, false.</returns>
    public bool HasValidMoves(Board board, Player player)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var position = new Position(row, col);
                if (IsValidMove(board, position, player))
                {
                    return true;
                }
            }
        }

        return false;
    }
}


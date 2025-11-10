namespace Othello.GameLogic;

/// <summary>
/// Represents the 8 possible directions on the board.
/// </summary>
public readonly record struct Direction(int RowDelta, int ColumnDelta)
{
    /// <summary>
    /// North direction (up).
    /// </summary>
    public static readonly Direction North = new(-1, 0);
    
    /// <summary>
    /// South direction (down).
    /// </summary>
    public static readonly Direction South = new(1, 0);
    
    /// <summary>
    /// East direction (right).
    /// </summary>
    public static readonly Direction East = new(0, 1);
    
    /// <summary>
    /// West direction (left).
    /// </summary>
    public static readonly Direction West = new(0, -1);
    
    /// <summary>
    /// NorthEast direction (up-right).
    /// </summary>
    public static readonly Direction NorthEast = new(-1, 1);
    
    /// <summary>
    /// NorthWest direction (up-left).
    /// </summary>
    public static readonly Direction NorthWest = new(-1, -1);
    
    /// <summary>
    /// SouthEast direction (down-right).
    /// </summary>
    public static readonly Direction SouthEast = new(1, 1);
    
    /// <summary>
    /// SouthWest direction (down-left).
    /// </summary>
    public static readonly Direction SouthWest = new(1, -1);

    /// <summary>
    /// Gets all 8 directions.
    /// </summary>
    public static IEnumerable<Direction> All
    {
        get
        {
            yield return North;
            yield return South;
            yield return East;
            yield return West;
            yield return NorthEast;
            yield return NorthWest;
            yield return SouthEast;
            yield return SouthWest;
        }
    }

    /// <summary>
    /// Gets the next position from the current position in this direction.
    /// </summary>
    /// <param name="position">The current position.</param>
    /// <returns>The next position, or null if it would be outside the board.</returns>
    public Position? GetNext(Position position)
    {
        int newRow = position.Row + RowDelta;
        int newCol = position.Column + ColumnDelta;
        
        if (!Position.IsValid(newRow, newCol))
            return null;
        
        return new Position(newRow, newCol);
    }
}


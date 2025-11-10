namespace Othello.GameLogic;

/// <summary>
/// Represents a position on the Othello board.
/// </summary>
public readonly record struct Position
{
    /// <summary>
    /// Gets the row coordinate (0-7).
    /// </summary>
    public int Row { get; }
    
    /// <summary>
    /// Gets the column coordinate (0-7).
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Initializes a new instance of the Position struct with validation.
    /// </summary>
    /// <param name="row">The row coordinate (0-7).</param>
    /// <param name="column">The column coordinate (0-7).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is outside the range 0-7.</exception>
    public Position(int row, int column)
    {
        if (row < 0 || row > 7)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 7.");
        if (column < 0 || column > 7)
            throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and 7.");
        
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Checks if the position is within the board boundaries.
    /// </summary>
    public static bool IsValid(int row, int column) => row >= 0 && row <= 7 && column >= 0 && column <= 7;
}


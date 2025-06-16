namespace ConWin.Lib;

/// <summary>
/// Represents a position with X and Y coordinates.
/// </summary>
public record Position(int X, int Y)
{
    /// <summary>
    /// Origin position (0, 0).
    /// </summary>
    public static readonly Position Origin = new(0, 0);
} 
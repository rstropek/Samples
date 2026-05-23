namespace BestSeating.Logic;

public enum LeftRight
{
    Left,
    Right
}

/// <summary>
/// Represents a seat in a theater.
/// </summary>
/// <param name="RowIx">Zero-based row index (row 0 is nearest the stage)</param>
/// <param name="ColIx">Zero-based column index (column 0 is the leftmost seat when facing the stage)</param>
/// <param name="Category">The category of the seat (lower values indicate better seats)</param>
/// <param name="Row">The row name (e.g. "F1", "C2", "10", "W")</param>
/// <param name="Side">The side of the theater (left or right)</param>
/// <param name="SeatNumber">The seat number within the row</param>
public record Seat(
    int RowIx, 
    int ColIx, 
    int Category,
    string Row, 
    LeftRight Side, 
    int SeatNumber
);

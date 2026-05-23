namespace BestSeating.Logic;

/// <summary>
/// Seat generator for the main theatre layout as specified in the documentation.
/// </summary>
public class TheatreSeatGenerator : ISeatGenerator
{
    public IEnumerable<Seat> Generate()
    {
        // Fauteuil (F1) - Category 1, Row 1, 9 seats per side
        foreach (var seat in GenerateRow(rowIx: 0, "F1", category: 1, seatsPerSide: 9))
            yield return seat;
        
        // Circle (C1, C2) - Category 2, Rows 2-3, 11 seats per side
        foreach (var seat in GenerateRows(startRowIx: 1, rowCount: 2, rowPrefix: "C", category: 2, seatsPerSide: 11))
            yield return seat;
        
        // Category 3 - Rows 1-5, 11 seats per side
        foreach (var seat in GenerateRows(startRowIx: 3, rowCount: 5, rowPrefix: "", category: 3, seatsPerSide: 11))
            yield return seat;
        
        // Category 4 - Rows 6-9, 11 seats per side
        foreach (var seat in GenerateRows(startRowIx: 8, rowCount: 4, rowPrefix: "", category: 4, seatsPerSide: 11))
            yield return seat;
        
        // Category 5 - Rows 10-12, 11 seats per side
        foreach (var seat in GenerateRows(startRowIx: 12, rowCount: 3, rowPrefix: "", category: 5, seatsPerSide: 11))
            yield return seat;
        
        // Category 6 - Rows 13-15, 10 seats per side
        foreach (var seat in GenerateRows(startRowIx: 15, rowCount: 3, rowPrefix: "", category: 6, seatsPerSide: 10))
            yield return seat;
        
        // Category 7 - Rows 16-17, 9 seats per side
        foreach (var seat in GenerateRows(startRowIx: 18, rowCount: 2, rowPrefix: "", category: 7, seatsPerSide: 9))
            yield return seat;
        
        // Category 8 - Rows 18-19, 9 seats on left and 6 seats on right
        foreach (var seat in GenerateRow(rowIx: 20, "18", category: 8, leftSeats: 9, rightSeats: 6))
            yield return seat;
        foreach (var seat in GenerateRow(rowIx: 21, "19", category: 8, leftSeats: 9, rightSeats: 6))
            yield return seat;
        
        // Wheelchair spots - 5 seats, all on the right side
        for (int i = 0; i < 5; i++)
        {
            yield return new Seat(
                RowIx: 22,
                ColIx: i,
                Category: 3, // Assuming same category as middle rows
                Row: "W",
                Side: LeftRight.Right,
                SeatNumber: i + 1
            );
        }
    }

    private IEnumerable<Seat> GenerateRows(int startRowIx, int rowCount, string rowPrefix, int category, int seatsPerSide)
    {
        for (int i = 0; i < rowCount; i++)
        {
            int rowIx = startRowIx + i;
            string rowName = rowPrefix + (rowPrefix == "" ? (i + 1).ToString() : (i + 1).ToString());
            
            foreach (var seat in GenerateRow(rowIx, rowName, category, seatsPerSide))
                yield return seat;
        }
    }

    private IEnumerable<Seat> GenerateRow(int rowIx, string rowName, int category, int seatsPerSide)
    {
        return GenerateRow(rowIx, rowName, category, seatsPerSide, seatsPerSide);
    }

    private IEnumerable<Seat> GenerateRow(int rowIx, string rowName, int category, int leftSeats, int rightSeats)
    {
        // Left side seats
        for (int i = 0; i < leftSeats; i++)
        {
            yield return new Seat(
                RowIx: rowIx,
                ColIx: i,
                Category: category,
                Row: rowName,
                Side: LeftRight.Left,
                SeatNumber: leftSeats - i // Seats numbered from center to wall
            );
        }

        // Right side seats
        for (int i = 0; i < rightSeats; i++)
        {
            yield return new Seat(
                RowIx: rowIx,
                ColIx: leftSeats + i, // Continue column indexing from left side
                Category: category,
                Row: rowName,
                Side: LeftRight.Right,
                SeatNumber: i + 1 // Seats numbered from center to wall
            );
        }
    }
}

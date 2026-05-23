namespace BestSeating.Logic;

/// <summary>
/// Interface for seat generators that create seat objects for theatres.
/// </summary>
public interface ISeatGenerator
{
    /// <summary>
    /// Generates an enumerable collection of seats.
    /// </summary>
    /// <returns>An enumerable of Seat objects.</returns>
    IEnumerable<Seat> Generate();
}

namespace TrainStation;

/// <summary>
/// Represents a train station.
/// </summary>
/// <remarks>
/// Our train station has 10 tracks, numbered 1 through 10.
/// Tracks 1 through 8 can be entered from the east or the west.
/// Tracks 9 and 10 can only be entered from the west.
/// </remarks>
class TrainStation
{
    internal Track[] Tracks { get; } = new Track[10];

    public TrainStation()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Gets the track with the given track number.
    /// </summary>
    /// <param name="number">Track number (1-10)</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <c>number</c> must be between 1 and 10.
    /// </exception>
    public Track GetTrack(int number)
    {
        throw new NotImplementedException();
    }
}

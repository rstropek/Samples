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
        for (int i = 0; i < 8; i++)
        {
            Tracks[i] = new Track(TrackAccess.Both, 10);
        }
        Tracks[8] = new Track(TrackAccess.West, 10);
        Tracks[9] = new Track(TrackAccess.West, 10);
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
        if (number is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Track number must be between 1 and 10.");
        }

        return Tracks[number - 1];
    }
}

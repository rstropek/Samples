namespace TrainStation.Tests;

public class TrainStationTests
{
    [Fact]
    public void Constructor_CreatesCorrectNumberOfTracks()
    {
        var station = new TrainStation();

        Assert.Equal(10, station.Tracks.Length);
    }

    [Fact]
    public void Constructor_CreatesCorrectTrackAccess()
    {
        var station = new TrainStation();

        for (int i = 0; i < 8; i++)
        {
            Assert.Equal(TrackAccess.Both, station.Tracks[i].Access);
        }
        Assert.Equal(TrackAccess.West, station.Tracks[8].Access);
        Assert.Equal(TrackAccess.West, station.Tracks[9].Access);
    }

    [Fact]
    public void GetTrack_ReturnsCorrectTrack()
    {
        var station = new TrainStation();

        var track = station.GetTrack(1);

        Assert.Equal(station.Tracks[0], track);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GetTrack_ThrowsException_WhenNumberIsOutOfRange(int number)
    {
        var station = new TrainStation();

        Assert.Throws<ArgumentOutOfRangeException>(() => station.GetTrack(number));
    }
}
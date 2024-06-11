namespace TrainStation.Tests;

public class AddWagonTests
{
    [Fact]
    public void AddWagon_AddsWagonToEast_WhenAccessIsEastOrBoth()
    {
        var track = new Track(TrackAccess.East, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");

        track.AddWagon(wagon, Direction.East);

        Assert.Equal(1, track.NumberOfWagons);
    }

    [Fact]
    public void AddWagon_AddsWagonToWest_WhenAccessIsWestOrBoth()
    {
        var track = new Track(TrackAccess.West, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");

        track.AddWagon(wagon, Direction.West);

        Assert.Equal(1, track.NumberOfWagons);
    }

    [Fact]
    public void AddWagon_ThrowsException_WhenAccessIsNotEast()
    {
        var track = new Track(TrackAccess.West, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");

        Assert.Throws<InvalidOperationException>(() => track.AddWagon(wagon, Direction.East));
    }

    [Fact]
    public void AddWagon_ThrowsException_WhenAccessIsNotWest()
    {
        var track = new Track(TrackAccess.East, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");

        Assert.Throws<InvalidOperationException>(() => track.AddWagon(wagon, Direction.West));
    }

    [Fact]
    public void AddWagon_ThrowsException_WhenTrackIsFull()
    {
        var track = new Track(TrackAccess.Both, 1);
        var wagon1 = new Wagon(WagonType.Passenger, "abc");
        var wagon2 = new Wagon(WagonType.Passenger, "def");

        track.AddWagon(wagon1, Direction.East);

        Assert.Throws<InvalidOperationException>(() => track.AddWagon(wagon2, Direction.East));
    }
}
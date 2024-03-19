namespace TrainStation.Tests;

public class RemoveWagonTests
{
    [Fact]
    public void RemoveWagon_RemovesWagonFromEast_WhenAccessIsEastOrBoth()
    {
        var track = new Track(TrackAccess.East, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        track.AddWagon(wagon, Direction.East);

        track.RemoveWagon(Direction.East);

        Assert.Equal(0, track.NumberOfWagons);
    }

    [Fact]
    public void RemoveWagon_RemovesWagonFromWest_WhenAccessIsWestOrBoth()
    {
        var track = new Track(TrackAccess.West, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        track.AddWagon(wagon, Direction.West);

        track.RemoveWagon(Direction.West);

        Assert.Equal(0, track.NumberOfWagons);
    }

    [Fact]
    public void RemoveWagon_ThrowsException_WhenAccessIsNotEast()
    {
        var track = new Track(TrackAccess.West, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        track.AddWagon(wagon, Direction.West);

        Assert.Throws<InvalidOperationException>(() => track.RemoveWagon(Direction.East));
    }

    [Fact]
    public void RemoveWagon_ThrowsException_WhenAccessIsNotWest()
    {
        var track = new Track(TrackAccess.East, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        track.AddWagon(wagon, Direction.East);

        Assert.Throws<InvalidOperationException>(() => track.RemoveWagon(Direction.West));
    }

    [Fact]
    public void RemoveWagon_ThrowsException_WhenTrackIsEmpty()
    {
        var track = new Track(TrackAccess.Both, 1);

        Assert.Throws<InvalidOperationException>(() => track.RemoveWagon(Direction.East));
        Assert.Throws<InvalidOperationException>(() => track.RemoveWagon(Direction.West));
    }
}
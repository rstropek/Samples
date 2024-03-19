namespace TrainStation.Tests;

public class LeaveTests
{
    [Fact]
    public void Leave_EmptiesTrack_WhenAccessIsEastOrBoth_AndTrainHasLocomotive()
    {
        var track = new Track(TrackAccess.East, 2);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        var locomotive = new Wagon(WagonType.Locomotive, "abc");
        track.AddWagon(wagon, Direction.East);
        track.AddWagon(locomotive, Direction.East);

        track.Leave(Direction.East);

        Assert.Equal(0, track.NumberOfWagons);
    }

    [Fact]
    public void Leave_EmptiesTrack_WhenAccessIsWestOrBoth_AndTrainHasLocomotive()
    {
        var track = new Track(TrackAccess.West, 2);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        var locomotive = new Wagon(WagonType.Locomotive, "abc");
        track.AddWagon(wagon, Direction.West);
        track.AddWagon(locomotive, Direction.West);

        track.Leave(Direction.West);

        Assert.Equal(0, track.NumberOfWagons);
    }

    [Fact]
    public void Leave_ThrowsException_WhenAccessIsNotEast()
    {
        var track = new Track(TrackAccess.West, 1);
        var locomotive = new Wagon(WagonType.Locomotive, "abc");
        track.AddWagon(locomotive, Direction.West);

        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.East));
    }

    [Fact]
    public void Leave_ThrowsException_WhenAccessIsNotWest()
    {
        var track = new Track(TrackAccess.East, 1);
        var locomotive = new Wagon(WagonType.Locomotive, "abc");
        track.AddWagon(locomotive, Direction.East);

        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.West));
    }

    [Fact]
    public void Leave_ThrowsException_WhenTrackIsEmpty()
    {
        var track = new Track(TrackAccess.Both, 1);

        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.East));
        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.West));
    }

    [Fact]
    public void Leave_ThrowsException_WhenTrainDoesNotContainLocomotive()
    {
        var track = new Track(TrackAccess.Both, 1);
        var wagon = new Wagon(WagonType.Passenger, "abc");
        track.AddWagon(wagon, Direction.East);

        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.East));
        Assert.Throws<InvalidOperationException>(() => track.Leave(Direction.West));
    }
}
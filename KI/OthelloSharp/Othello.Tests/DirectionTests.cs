using Othello.GameLogic;

namespace Othello.Tests;

public class DirectionTests
{
    [Fact]
    public void GetNext_North_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.North.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(3, next.Value.Row);
        Assert.Equal(4, next.Value.Column);
    }

    [Fact]
    public void GetNext_South_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.South.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(5, next.Value.Row);
        Assert.Equal(4, next.Value.Column);
    }

    [Fact]
    public void GetNext_East_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.East.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(4, next.Value.Row);
        Assert.Equal(5, next.Value.Column);
    }

    [Fact]
    public void GetNext_West_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.West.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(4, next.Value.Row);
        Assert.Equal(3, next.Value.Column);
    }

    [Fact]
    public void GetNext_NorthEast_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.NorthEast.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(3, next.Value.Row);
        Assert.Equal(5, next.Value.Column);
    }

    [Fact]
    public void GetNext_NorthWest_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.NorthWest.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(3, next.Value.Row);
        Assert.Equal(3, next.Value.Column);
    }

    [Fact]
    public void GetNext_SouthEast_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.SouthEast.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(5, next.Value.Row);
        Assert.Equal(5, next.Value.Column);
    }

    [Fact]
    public void GetNext_SouthWest_ReturnsCorrectPosition()
    {
        // Arrange
        var position = new Position(4, 4);
        
        // Act
        var next = Direction.SouthWest.GetNext(position);
        
        // Assert
        Assert.NotNull(next);
        Assert.Equal(5, next.Value.Row);
        Assert.Equal(3, next.Value.Column);
    }

    [Fact]
    public void GetNext_OutOfBoundsNorth_ReturnsNull()
    {
        // Arrange
        var position = new Position(0, 4);
        
        // Act
        var next = Direction.North.GetNext(position);
        
        // Assert
        Assert.Null(next);
    }

    [Fact]
    public void GetNext_OutOfBoundsSouth_ReturnsNull()
    {
        // Arrange
        var position = new Position(7, 4);
        
        // Act
        var next = Direction.South.GetNext(position);
        
        // Assert
        Assert.Null(next);
    }

    [Fact]
    public void GetNext_OutOfBoundsEast_ReturnsNull()
    {
        // Arrange
        var position = new Position(4, 7);
        
        // Act
        var next = Direction.East.GetNext(position);
        
        // Assert
        Assert.Null(next);
    }

    [Fact]
    public void GetNext_OutOfBoundsWest_ReturnsNull()
    {
        // Arrange
        var position = new Position(4, 0);
        
        // Act
        var next = Direction.West.GetNext(position);
        
        // Assert
        Assert.Null(next);
    }

    [Fact]
    public void GetNext_OutOfBoundsCorner_ReturnsNull()
    {
        // Arrange
        var position = new Position(0, 0);
        
        // Act
        var northWest = Direction.NorthWest.GetNext(position);
        var north = Direction.North.GetNext(position);
        var west = Direction.West.GetNext(position);
        
        // Assert
        Assert.Null(northWest);
        Assert.Null(north);
        Assert.Null(west);
    }

    [Fact]
    public void All_ReturnsAllEightDirections()
    {
        // Act
        var directions = Direction.All.ToList();
        
        // Assert
        Assert.Equal(8, directions.Count);
        Assert.Contains(Direction.North, directions);
        Assert.Contains(Direction.South, directions);
        Assert.Contains(Direction.East, directions);
        Assert.Contains(Direction.West, directions);
        Assert.Contains(Direction.NorthEast, directions);
        Assert.Contains(Direction.NorthWest, directions);
        Assert.Contains(Direction.SouthEast, directions);
        Assert.Contains(Direction.SouthWest, directions);
    }
}


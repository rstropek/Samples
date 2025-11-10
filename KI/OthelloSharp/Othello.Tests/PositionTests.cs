using Othello.GameLogic;

namespace Othello.Tests;

public class PositionTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 4)]
    [InlineData(7, 7)]
    [InlineData(0, 7)]
    [InlineData(7, 0)]
    public void Constructor_ValidCoordinates_CreatesPosition(int row, int column)
    {
        // Act
        var position = new Position(row, column);
        
        // Assert
        Assert.Equal(row, position.Row);
        Assert.Equal(column, position.Column);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(8, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 8)]
    [InlineData(-1, -1)]
    [InlineData(10, 10)]
    public void Constructor_InvalidCoordinates_ThrowsArgumentOutOfRangeException(int row, int column)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Position(row, column));
    }

    [Fact]
    public void Equality_SameCoordinates_AreEqual()
    {
        // Arrange
        var position1 = new Position(3, 4);
        var position2 = new Position(3, 4);
        
        // Act & Assert
        Assert.Equal(position1, position2);
        Assert.True(position1 == position2);
    }

    [Fact]
    public void Equality_DifferentCoordinates_AreNotEqual()
    {
        // Arrange
        var position1 = new Position(3, 4);
        var position2 = new Position(4, 3);
        
        // Act & Assert
        Assert.NotEqual(position1, position2);
        Assert.True(position1 != position2);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(7, 7, true)]
    [InlineData(3, 4, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(8, 0, false)]
    [InlineData(0, 8, false)]
    public void IsValid_ReturnsCorrectResult(int row, int column, bool expected)
    {
        // Act
        var result = Position.IsValid(row, column);
        
        // Assert
        Assert.Equal(expected, result);
    }
}


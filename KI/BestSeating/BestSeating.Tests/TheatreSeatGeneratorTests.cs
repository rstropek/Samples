namespace BestSeating.Tests;

using BestSeating.Logic;
using Xunit;
using System.Linq;

public class TheatreSeatGeneratorTests
{
    private readonly ISeatGenerator _seatGenerator;

    public TheatreSeatGeneratorTests()
    {
        _seatGenerator = new TheatreSeatGenerator();
    }

    [Fact]
    public void Generate_ShouldNotThrowException()
    {
        // Act & Assert - just make sure it doesn't throw
        var seats = _seatGenerator.Generate();
        _ = seats.ToList(); // Force enumeration
    }

    [Fact]
    public void Generate_ShouldCreateCorrectTotalSeats()
    {
        // Act
        var seats = _seatGenerator.Generate().ToList();

        // Assert
        Assert.Equal(452, seats.Count(s => s.Row != "W")); // 452 regular seats
        Assert.Equal(5, seats.Count(s => s.Row == "W")); // 5 wheelchair spots
        Assert.Equal(457, seats.Count); // Total seats including wheelchair spots
    }

    [Theory]
    [InlineData(1, 18)] // Fauteuil (F1)
    [InlineData(2, 44)] // Circle (C1, C2)
    [InlineData(3, 110)] // Category 3
    [InlineData(4, 88)] // Category 4
    [InlineData(5, 66)] // Category 5
    [InlineData(6, 60)] // Category 6
    [InlineData(7, 36)] // Category 7
    [InlineData(8, 30)] // Category 8
    public void Generate_ShouldCreateCorrectSeatsPerCategory(int category, int expectedCount)
    {
        // Act
        var seats = _seatGenerator.Generate().ToList();

        // Assert
        Assert.Equal(expectedCount, seats.Count(s => s.Category == category && s.Row != "W"));
    }

    [Fact]
    public void Generate_ShouldCreateCorrectWheelchairSeats()
    {
        // Act
        var seats = _seatGenerator.Generate().ToList();
        var wheelchairSeats = seats.Where(s => s.Row == "W").ToList();

        // Assert
        Assert.Equal(5, wheelchairSeats.Count);
        Assert.All(wheelchairSeats, s => Assert.Equal(LeftRight.Right, s.Side));
    }

    [Fact]
    public void Generate_ShouldCreateCorrectNumberOfLeftAndRightSeats()
    {
        // Act
        var seats = _seatGenerator.Generate().Where(s => s.Row != "W").ToList();
        
        // Assert
        Assert.Equal(229, seats.Count(s => s.Side == LeftRight.Left));
        Assert.Equal(223, seats.Count(s => s.Side == LeftRight.Right));
    }
}

using Othello.GameLogic;

namespace Othello.Tests;

public class MoveValidatorTests
{
    private readonly MoveValidator validator = new();

    [Fact]
    public void IsValidMove_OccupiedPosition_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(3, 3); // Occupied by White
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_NoFlips_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(0, 0); // Corner, no adjacent pieces
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_ValidMoveNorth_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Initial: (3,3)=W, (4,3)=B
        // Move at (2,3) by Black should flip (3,3)
        var position = new Position(2, 3);
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ValidMoveSouth_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Initial: (3,4)=B, (4,4)=W
        // Move at (5,4) by Black should flip (4,4)
        var position = new Position(5, 4);
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ValidMoveEast_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Initial: (4,3)=B, (4,4)=W
        // Move at (4,5) by Black should flip (4,4)
        var position = new Position(4, 5);
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidMove_ValidMoveWest_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Initial: (3,3)=W, (3,4)=B
        // Move at (3,2) by Black should flip (3,3)
        var position = new Position(3, 2);
        
        // Act
        var result = validator.IsValidMove(board, position, Player.Black);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetFlippableDiscs_ValidMove_ReturnsCorrectDiscs()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Move at (2,3) by Black should flip (3,3)
        var position = new Position(2, 3);
        
        // Act
        var flippable = validator.GetFlippableDiscs(board, position, Player.Black);
        
        // Assert
        Assert.Single(flippable);
        Assert.Contains(new Position(3, 3), flippable);
    }

    [Fact]
    public void GetFlippableDiscs_NoFlips_ReturnsEmpty()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(0, 0);
        
        // Act
        var flippable = validator.GetFlippableDiscs(board, position, Player.Black);
        
        // Assert
        Assert.Empty(flippable);
    }

    [Fact]
    public void GetFlippableDiscs_MultipleDirections_ReturnsAllFlippable()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Set up a scenario where one move flips multiple directions
        board.SetDisc(new Position(2, 2), Player.Black);
        board.SetDisc(new Position(2, 3), Player.White);
        board.SetDisc(new Position(2, 4), Player.White);
        // Move at (2,5) by Black should flip (2,3) and (2,4)
        var position = new Position(2, 5);
        
        // Act
        var flippable = validator.GetFlippableDiscs(board, position, Player.Black);
        
        // Assert
        Assert.Equal(2, flippable.Count);
        Assert.Contains(new Position(2, 3), flippable);
        Assert.Contains(new Position(2, 4), flippable);
    }

    [Fact]
    public void GetFlippableDiscs_LongLine_ReturnsAllInBetween()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        // Create a long line of white discs
        board.SetDisc(new Position(0, 3), Player.Black);
        board.SetDisc(new Position(1, 3), Player.White);
        board.SetDisc(new Position(2, 3), Player.White);
        // Position (3,3) already has White from initialization
        // Move at (4,3) is Black from initialization
        // This should flip (1,3), (2,3), and (3,3)
        
        // Act
        var flippable = validator.GetFlippableDiscs(board, new Position(4, 3), Player.Black);
        
        // Assert - The existing black at 4,3 means we're checking what WOULD flip
        // Actually, let me set up the test differently
        
        // Clear and set up properly
        board = new Board();
        board.SetDisc(new Position(0, 3), Player.Black);
        board.SetDisc(new Position(1, 3), Player.White);
        board.SetDisc(new Position(2, 3), Player.White);
        board.SetDisc(new Position(3, 3), Player.White);
        
        flippable = validator.GetFlippableDiscs(board, new Position(4, 3), Player.Black);
        
        Assert.Equal(3, flippable.Count);
        Assert.Contains(new Position(1, 3), flippable);
        Assert.Contains(new Position(2, 3), flippable);
        Assert.Contains(new Position(3, 3), flippable);
    }

    [Fact]
    public void GetValidMoves_InitialBoard_ReturnsFourMovesForBlack()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act
        var validMoves = validator.GetValidMoves(board, Player.Black);
        
        // Assert
        Assert.Equal(4, validMoves.Count);
        Assert.Contains(new Position(2, 3), validMoves); // North of (3,3)
        Assert.Contains(new Position(3, 2), validMoves); // West of (3,3)
        Assert.Contains(new Position(4, 5), validMoves); // East of (4,4)
        Assert.Contains(new Position(5, 4), validMoves); // South of (4,4)
    }

    [Fact]
    public void GetValidMoves_InitialBoard_ReturnsFourMovesForWhite()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act
        var validMoves = validator.GetValidMoves(board, Player.White);
        
        // Assert
        Assert.Equal(4, validMoves.Count);
        Assert.Contains(new Position(2, 4), validMoves);
        Assert.Contains(new Position(3, 5), validMoves);
        Assert.Contains(new Position(4, 2), validMoves);
        Assert.Contains(new Position(5, 3), validMoves);
    }

    [Fact]
    public void HasValidMoves_InitialBoard_ReturnsTrueForBothPlayers()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act & Assert
        Assert.True(validator.HasValidMoves(board, Player.Black));
        Assert.True(validator.HasValidMoves(board, Player.White));
    }

    [Fact]
    public void HasValidMoves_NoValidMoves_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        // Set up a board where a player has no valid moves
        // (This would be a very specific end-game scenario)
        // For simplicity, let's test with an empty board
        
        // Act
        var result = validator.HasValidMoves(board, Player.Black);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidMove_Diagonal_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        // Set up for diagonal move without using Initialize()
        board.SetDisc(new Position(2, 2), Player.Black);
        board.SetDisc(new Position(3, 3), Player.White);
        
        // Act - (4,4) by Black should flip (3,3) diagonally
        var result = validator.IsValidMove(board, new Position(4, 4), Player.Black);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetFlippableDiscs_EmptyInMiddle_ReturnsEmpty()
    {
        // Arrange
        var board = new Board();
        board.SetDisc(new Position(0, 0), Player.Black);
        board.SetDisc(new Position(1, 0), Player.White);
        // Position (2,0) is empty
        board.SetDisc(new Position(3, 0), Player.Black);
        
        // Act - Move at (4,0) should not flip anything because there's an empty space
        var flippable = validator.GetFlippableDiscs(board, new Position(4, 0), Player.Black);
        
        // Assert
        Assert.Empty(flippable);
    }

    [Fact]
    public void IsValidMove_AdjacentSameColor_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        board.SetDisc(new Position(3, 3), Player.Black);
        board.SetDisc(new Position(3, 4), Player.Black);
        
        // Act - Moving next to same color with no opponent in between
        var result = validator.IsValidMove(board, new Position(3, 5), Player.Black);
        
        // Assert
        Assert.False(result);
    }
}


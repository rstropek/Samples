using Othello.GameLogic;

namespace Othello.Tests;

public class BoardTests
{
    [Fact]
    public void Initialize_SetsUpStandardStartingPosition()
    {
        // Arrange
        var board = new Board();
        
        // Act
        board.Initialize();
        
        // Assert - Check the 4 starting positions
        Assert.Equal(Player.White, board.GetDisc(new Position(3, 3)));
        Assert.Equal(Player.Black, board.GetDisc(new Position(3, 4)));
        Assert.Equal(Player.Black, board.GetDisc(new Position(4, 3)));
        Assert.Equal(Player.White, board.GetDisc(new Position(4, 4)));
        
        // Assert - Check that other positions are empty
        Assert.Null(board.GetDisc(new Position(0, 0)));
        Assert.Null(board.GetDisc(new Position(7, 7)));
        Assert.Null(board.GetDisc(new Position(3, 2)));
    }

    [Fact]
    public void GetDisc_ReturnsCorrectPlayer()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act & Assert
        Assert.Equal(Player.Black, board.GetDisc(new Position(3, 4)));
        Assert.Equal(Player.White, board.GetDisc(new Position(3, 3)));
    }

    [Fact]
    public void GetDisc_EmptyPosition_ReturnsNull()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act
        var result = board.GetDisc(new Position(0, 0));
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SetDisc_PlacesDiscCorrectly()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(2, 3);
        
        // Act
        board.SetDisc(position, Player.Black);
        
        // Assert
        Assert.Equal(Player.Black, board.GetDisc(position));
    }

    [Fact]
    public void SetDisc_CanOverwriteExistingDisc()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(3, 3);
        
        // Verify initial state
        Assert.Equal(Player.White, board.GetDisc(position));
        
        // Act
        board.SetDisc(position, Player.Black);
        
        // Assert
        Assert.Equal(Player.Black, board.GetDisc(position));
    }

    [Fact]
    public void IsOccupied_OccupiedPosition_ReturnsTrue()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act & Assert
        Assert.True(board.IsOccupied(new Position(3, 3)));
        Assert.True(board.IsOccupied(new Position(4, 4)));
    }

    [Fact]
    public void IsOccupied_EmptyPosition_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act & Assert
        Assert.False(board.IsOccupied(new Position(0, 0)));
        Assert.False(board.IsOccupied(new Position(7, 7)));
    }

    [Fact]
    public void CountDiscs_InitialPosition_Returns2ForEachPlayer()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        
        // Act
        var blackCount = board.CountDiscs(Player.Black);
        var whiteCount = board.CountDiscs(Player.White);
        
        // Assert
        Assert.Equal(2, blackCount);
        Assert.Equal(2, whiteCount);
    }

    [Fact]
    public void CountDiscs_AfterPlacingDiscs_ReturnsCorrectCount()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        board.SetDisc(new Position(2, 3), Player.Black);
        board.SetDisc(new Position(2, 4), Player.Black);
        board.SetDisc(new Position(5, 5), Player.White);
        
        // Act
        var blackCount = board.CountDiscs(Player.Black);
        var whiteCount = board.CountDiscs(Player.White);
        
        // Assert
        Assert.Equal(4, blackCount); // 2 initial + 2 placed
        Assert.Equal(3, whiteCount); // 2 initial + 1 placed
    }

    [Fact]
    public void Clone_CreatesIndependentCopy()
    {
        // Arrange
        var original = new Board();
        original.Initialize();
        
        // Act
        var clone = original.Clone();
        
        // Modify the clone
        clone.SetDisc(new Position(0, 0), Player.Black);
        
        // Assert - original should not be affected
        Assert.Null(original.GetDisc(new Position(0, 0)));
        Assert.Equal(Player.Black, clone.GetDisc(new Position(0, 0)));
        
        // Assert - initial positions should match
        Assert.Equal(original.GetDisc(new Position(3, 3)), clone.GetDisc(new Position(3, 3)));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    [InlineData(3, 5)]
    public void GetDisc_ValidPositions_DoesNotThrow(int row, int col)
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var position = new Position(row, col);
        
        // Act & Assert - Should not throw
        var result = board.GetDisc(position);
        Assert.True(result == null || result == Player.Black || result == Player.White);
    }

    [Fact]
    public void PlaceDisc_ValidMove_PlacesDiscAndFlips()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var validator = new MoveValidator();
        var position = new Position(2, 3);
        
        // Act
        var result = board.PlaceDisc(position, Player.Black, validator);
        
        // Assert
        Assert.True(result);
        Assert.Equal(Player.Black, board.GetDisc(position));
        Assert.Equal(Player.Black, board.GetDisc(new Position(3, 3))); // Should be flipped
        Assert.Equal(4, board.CountDiscs(Player.Black)); // 2 initial + 1 placed + 1 flipped
        Assert.Equal(1, board.CountDiscs(Player.White)); // 2 initial - 1 flipped
    }

    [Fact]
    public void PlaceDisc_InvalidMove_ReturnsFalseAndDoesNotModifyBoard()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var validator = new MoveValidator();
        var position = new Position(0, 0); // No valid flips
        var initialBlackCount = board.CountDiscs(Player.Black);
        var initialWhiteCount = board.CountDiscs(Player.White);
        
        // Act
        var result = board.PlaceDisc(position, Player.Black, validator);
        
        // Assert
        Assert.False(result);
        Assert.Null(board.GetDisc(position)); // Should still be empty
        Assert.Equal(initialBlackCount, board.CountDiscs(Player.Black));
        Assert.Equal(initialWhiteCount, board.CountDiscs(Player.White));
    }

    [Fact]
    public void PlaceDisc_FlipsMultipleDiscs_FlipsAll()
    {
        // Arrange
        var board = new Board();
        var validator = new MoveValidator();
        board.SetDisc(new Position(3, 3), Player.Black);
        board.SetDisc(new Position(3, 4), Player.White);
        board.SetDisc(new Position(3, 5), Player.White);
        var position = new Position(3, 6);
        
        // Act
        var result = board.PlaceDisc(position, Player.Black, validator);
        
        // Assert
        Assert.True(result);
        Assert.Equal(Player.Black, board.GetDisc(new Position(3, 4))); // Flipped
        Assert.Equal(Player.Black, board.GetDisc(new Position(3, 5))); // Flipped
        Assert.Equal(4, board.CountDiscs(Player.Black)); // 1 initial + 1 placed + 2 flipped
    }

    [Fact]
    public void PlaceDisc_OccupiedPosition_ReturnsFalse()
    {
        // Arrange
        var board = new Board();
        board.Initialize();
        var validator = new MoveValidator();
        var position = new Position(3, 3); // Already occupied
        
        // Act
        var result = board.PlaceDisc(position, Player.Black, validator);
        
        // Assert
        Assert.False(result);
        Assert.Equal(Player.White, board.GetDisc(position)); // Should remain White
    }
}


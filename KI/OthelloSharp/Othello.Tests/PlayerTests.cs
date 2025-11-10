using Othello.GameLogic;

namespace Othello.Tests;

public class PlayerTests
{
    [Fact]
    public void Opponent_BlackPlayer_ReturnsWhite()
    {
        // Arrange
        var player = Player.Black;
        
        // Act
        var opponent = player.Opponent();
        
        // Assert
        Assert.Equal(Player.White, opponent);
    }

    [Fact]
    public void Opponent_WhitePlayer_ReturnsBlack()
    {
        // Arrange
        var player = Player.White;
        
        // Act
        var opponent = player.Opponent();
        
        // Assert
        Assert.Equal(Player.Black, opponent);
    }
}


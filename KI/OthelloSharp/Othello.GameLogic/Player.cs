namespace Othello.GameLogic;

/// <summary>
/// Represents a player in the Othello game.
/// </summary>
public enum Player
{
    /// <summary>
    /// The Black player, who moves first.
    /// </summary>
    Black,
    
    /// <summary>
    /// The White player, who moves second.
    /// </summary>
    White
}

/// <summary>
/// Extension methods for the Player enum.
/// </summary>
public static class PlayerExtensions
{
    /// <summary>
    /// Gets the opponent of the current player.
    /// </summary>
    /// <param name="player">The current player.</param>
    /// <returns>The opponent player.</returns>
    public static Player Opponent(this Player player) => player == Player.Black ? Player.White : Player.Black;
}


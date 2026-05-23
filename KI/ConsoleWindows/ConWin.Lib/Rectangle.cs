namespace ConWin.Lib;

/// <summary>
/// Represents a rectangle with position and size.
/// </summary>
public record Rectangle(Position Position, Size Size)
{
    /// <summary>
    /// Gets the X coordinate of the left edge.
    /// </summary>
    public int Left => Position.X;

    /// <summary>
    /// Gets the Y coordinate of the top edge.
    /// </summary>
    public int Top => Position.Y;

    /// <summary>
    /// Gets the X coordinate of the right edge.
    /// </summary>
    public int Right => Position.X + Size.Width - 1;

    /// <summary>
    /// Gets the Y coordinate of the bottom edge.
    /// </summary>
    public int Bottom => Position.Y + Size.Height - 1;

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    /// Determines whether this rectangle completely contains another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to check.</param>
    /// <returns>True if this rectangle completely contains the other rectangle.</returns>
    public bool Contains(Rectangle other)
    {
        return Left <= other.Left && 
               Top <= other.Top && 
               Right >= other.Right && 
               Bottom >= other.Bottom;
    }

    /// <summary>
    /// Determines whether this rectangle overlaps with another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to check.</param>
    /// <returns>True if the rectangles overlap.</returns>
    public bool Overlaps(Rectangle other)
    {
        return Left < other.Right && 
               Right > other.Left && 
               Top < other.Bottom && 
               Bottom > other.Top;
    }
} 
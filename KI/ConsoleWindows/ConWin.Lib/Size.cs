namespace ConWin.Lib;

/// <summary>
/// Represents a size with Width and Height dimensions.
/// </summary>
public record Size(int Width, int Height)
{
    /// <summary>
    /// Empty size (0, 0).
    /// </summary>
    public static readonly Size Empty = new(0, 0);
    
    /// <summary>
    /// Gets whether this size is empty (both width and height are 0).
    /// </summary>
    public bool IsEmpty => Width == 0 && Height == 0;
    
    /// <summary>
    /// Gets the area of this size (Width * Height).
    /// </summary>
    public int Area => Width * Height;
} 
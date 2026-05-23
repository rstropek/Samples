namespace ConWin.Lib;

/// <summary>
/// Represents a window with title and border styling capabilities.
/// </summary>
public class Window : WindowBase
{
    private string _title = string.Empty;
    private BorderStyle _borderStyle = BorderStyle.None;

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    public string Title
    {
        get => _title;
        set => _title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the border style of the window.
    /// </summary>
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set => _borderStyle = value;
    }

    /// <summary>
    /// Initializes a new instance of the Window class.
    /// </summary>
    public Window()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Window class with the specified parameters.
    /// </summary>
    /// <param name="position">The position of the window.</param>
    /// <param name="size">The size of the window.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="borderStyle">The border style of the window.</param>
    public Window(Position position, Size size, string title = "", BorderStyle borderStyle = BorderStyle.None)
    {
        Position = position;
        Size = size;
        Title = title;
        BorderStyle = borderStyle;
    }

    /// <summary>
    /// Draws the window on the console screen.
    /// </summary>
    public override void Draw()
    {
        if (!IsVisible || Size.IsEmpty)
            return;

        // Set console colors
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;

        DrawBackground();
        DrawBorder();
        DrawTitle();
    }

    /// <summary>
    /// Draws the background of the window.
    /// </summary>
    private void DrawBackground()
    {
        for (int y = 0; y < Size.Height; y++)
        {
            Console.SetCursorPosition(Position.X, Position.Y + y);
            Console.Write(new string(' ', Size.Width));
        }
    }

    /// <summary>
    /// Draws the border of the window based on the border style.
    /// </summary>
    private void DrawBorder()
    {
        if (BorderStyle == BorderStyle.None || Size.Width < 2 || Size.Height < 2)
            return;

        char topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical;

        if (BorderStyle == BorderStyle.Single)
        {
            topLeft = '┌'; topRight = '┐'; bottomLeft = '└'; bottomRight = '┘';
            horizontal = '─'; vertical = '│';
        }
        else // BorderStyle.Double
        {
            topLeft = '╔'; topRight = '╗'; bottomLeft = '╚'; bottomRight = '╝';
            horizontal = '═'; vertical = '║';
        }

        // Draw top border
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(topLeft + new string(horizontal, Size.Width - 2) + topRight);

        // Draw side borders
        for (int y = 1; y < Size.Height - 1; y++)
        {
            Console.SetCursorPosition(Position.X, Position.Y + y);
            Console.Write(vertical);
            Console.SetCursorPosition(Position.X + Size.Width - 1, Position.Y + y);
            Console.Write(vertical);
        }

        // Draw bottom border
        if (Size.Height > 1)
        {
            Console.SetCursorPosition(Position.X, Position.Y + Size.Height - 1);
            Console.Write(bottomLeft + new string(horizontal, Size.Width - 2) + bottomRight);
        }
    }

    /// <summary>
    /// Draws the title of the window in the top border (if there is one).
    /// </summary>
    private void DrawTitle()
    {
        if (string.IsNullOrEmpty(Title) || BorderStyle == BorderStyle.None || Size.Width < 4)
            return;

        // Calculate title position and length
        int maxTitleLength = Size.Width - 4; // Leave space for border and padding
        string displayTitle = Title.Length > maxTitleLength ? Title[..maxTitleLength] : Title;
        int titleX = Position.X + 2; // Start after left border and one space

        Console.SetCursorPosition(titleX, Position.Y);
        Console.Write(displayTitle);
    }

    /// <summary>
    /// Gets the border offset (0 for no border, 1 for single/double border).
    /// </summary>
    /// <returns>The border offset.</returns>
    protected override int GetBorderOffset()
    {
        return BorderStyle == BorderStyle.None ? 0 : 1;
    }
} 
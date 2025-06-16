namespace ConWin.Lib;

/// <summary>
/// Abstract base class for all windows in the console window manager.
/// </summary>
public abstract class WindowBase
{
    private Position _position = Position.Origin;
    private Size _size = Size.Empty;
    private ConsoleColor _backgroundColor = ConsoleColor.Black;
    private ConsoleColor _foregroundColor = ConsoleColor.White;
    private int _zIndex = 0;
    private WindowBase? _parent;
    private readonly List<WindowBase> _children = new();

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    public Position Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    public Size Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>
    /// Gets or sets the background color of the window.
    /// </summary>
    public ConsoleColor BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    /// <summary>
    /// Gets or sets the foreground color of the window.
    /// </summary>
    public ConsoleColor ForegroundColor
    {
        get => _foregroundColor;
        set => _foregroundColor = value;
    }

    /// <summary>
    /// Gets or sets the z-index (drawing order) of the window.
    /// Higher values are drawn on top.
    /// </summary>
    public int ZIndex
    {
        get => _zIndex;
        set => _zIndex = value;
    }

    /// <summary>
    /// Gets or sets the parent window. Child windows are drawn on top of their parent.
    /// </summary>
    public WindowBase? Parent
    {
        get => _parent;
        set
        {
            if (_parent == value) return;
            
            _parent?.RemoveChild(this);
            _parent = value;
            _parent?.AddChild(this);
        }
    }

    /// <summary>
    /// Gets the collection of child windows.
    /// </summary>
    public IReadOnlyList<WindowBase> Children => _children.AsReadOnly();

    /// <summary>
    /// Gets whether this window is visible (not completely obscured by other windows).
    /// </summary>
    public bool IsVisible { get; internal set; } = true;

    /// <summary>
    /// Gets whether this window currently has focus.
    /// </summary>
    public bool HasFocus { get; internal set; } = false;

    /// <summary>
    /// Gets the bounds of the window (position and size combined).
    /// </summary>
    public Rectangle Bounds => new(Position, Size);

    /// <summary>
    /// Gets the client area bounds (interior area excluding border).
    /// </summary>
    public Rectangle ClientBounds
    {
        get
        {
            var borderOffset = GetBorderOffset();
            var clientSize = GetClientSize();
            return new Rectangle(
                new Position(Position.X + borderOffset, Position.Y + borderOffset),
                clientSize
            );
        }
    }

    /// <summary>
    /// Adds a child window to this window.
    /// </summary>
    /// <param name="child">The child window to add.</param>
    protected void AddChild(WindowBase child)
    {
        if (!_children.Contains(child))
        {
            _children.Add(child);
        }
    }

    /// <summary>
    /// Removes a child window from this window.
    /// </summary>
    /// <param name="child">The child window to remove.</param>
    protected void RemoveChild(WindowBase child)
    {
        _children.Remove(child);
    }

    /// <summary>
    /// Draws the window on the screen. Must be implemented by derived classes.
    /// </summary>
    public abstract void Draw();

    /// <summary>
    /// Handles a key press when this window has focus. Override in derived classes to provide custom key handling.
    /// </summary>
    /// <param name="keyInfo">Information about the key that was pressed.</param>
    /// <returns>True if the key was handled; otherwise, false.</returns>
    public virtual bool HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        return false; // Default implementation does not handle any keys
    }

    /// <summary>
    /// Sets the cursor position relative to the window's client area.
    /// </summary>
    /// <param name="x">The x-coordinate relative to the window's client area.</param>
    /// <param name="y">The y-coordinate relative to the window's client area.</param>
    /// <returns>True if the position is valid; otherwise, false.</returns>
    public bool SetCursorPosition(int x, int y)
    {
        var clientBounds = ClientBounds;
        
        if (x < 0 || y < 0 || x >= clientBounds.Size.Width || y >= clientBounds.Size.Height)
            return false;

        Console.SetCursorPosition(clientBounds.Position.X + x, clientBounds.Position.Y + y);
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
        return true;
    }

    /// <summary>
    /// Writes text to the window at the current cursor position, ensuring it stays within window bounds.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <returns>The number of characters actually written.</returns>
    public int Write(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        var clientBounds = ClientBounds;
        var currentPos = new Position(Console.CursorLeft, Console.CursorTop);
        
        // Check if cursor is within client bounds
        if (currentPos.X < clientBounds.Position.X || currentPos.Y < clientBounds.Position.Y ||
            currentPos.Y >= clientBounds.Position.Y + clientBounds.Size.Height)
            return 0;

        // Calculate how many characters can be written on the current line
        int maxCharsOnLine = clientBounds.Position.X + clientBounds.Size.Width - currentPos.X;
        if (maxCharsOnLine <= 0)
            return 0;

        // Truncate text if necessary
        string textToWrite = text.Length > maxCharsOnLine ? text[..maxCharsOnLine] : text;
        
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
        Console.Write(textToWrite);
        
        return textToWrite.Length;
    }

    /// <summary>
    /// Writes text to the window at the specified position relative to the client area.
    /// </summary>
    /// <param name="x">The x-coordinate relative to the window's client area.</param>
    /// <param name="y">The y-coordinate relative to the window's client area.</param>
    /// <param name="text">The text to write.</param>
    /// <returns>The number of characters actually written.</returns>
    public int WriteAt(int x, int y, string text)
    {
        if (!SetCursorPosition(x, y))
            return 0;
        
        return Write(text);
    }

    /// <summary>
    /// Writes a line of text to the window at the current cursor position.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <returns>The number of characters actually written.</returns>
    public int WriteLine(string text = "")
    {
        int written = Write(text);
        
        // Move to next line if possible
        var clientBounds = ClientBounds;
        var currentPos = new Position(Console.CursorLeft, Console.CursorTop);
        
        if (currentPos.Y + 1 < clientBounds.Position.Y + clientBounds.Size.Height)
        {
            SetCursorPosition(0, currentPos.Y - clientBounds.Position.Y + 1);
        }
        
        return written;
    }

    /// <summary>
    /// Writes a line of text to the window at the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate relative to the window's client area.</param>
    /// <param name="y">The y-coordinate relative to the window's client area.</param>
    /// <param name="text">The text to write.</param>
    /// <returns>The number of characters actually written.</returns>
    public int WriteLineAt(int x, int y, string text = "")
    {
        if (!SetCursorPosition(x, y))
            return 0;
        
        return WriteLine(text);
    }

    /// <summary>
    /// Clears the client area of the window.
    /// </summary>
    public void ClearClientArea()
    {
        var clientBounds = ClientBounds;
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
        
        for (int y = 0; y < clientBounds.Size.Height; y++)
        {
            Console.SetCursorPosition(clientBounds.Position.X, clientBounds.Position.Y + y);
            Console.Write(new string(' ', clientBounds.Size.Width));
        }
    }

    /// <summary>
    /// Gets the border offset (0 for no border, 1 for single/double border).
    /// </summary>
    /// <returns>The border offset.</returns>
    protected virtual int GetBorderOffset()
    {
        return 0; // Base class has no border
    }

    /// <summary>
    /// Gets the client size (interior size excluding border).
    /// </summary>
    /// <returns>The client size.</returns>
    protected virtual Size GetClientSize()
    {
        int borderOffset = GetBorderOffset();
        int clientWidth = Math.Max(0, Size.Width - (borderOffset * 2));
        int clientHeight = Math.Max(0, Size.Height - (borderOffset * 2));
        return new Size(clientWidth, clientHeight);
    }
} 
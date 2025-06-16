using ConWin.Lib;

Console.Title = "ConWin Framework Demo - Keyboard Handling";
Console.CursorVisible = false;

// Create the window manager
var windowManager = new WindowManager();

// Create demo windows
CreateDemoWindows(windowManager);

// Initial draw
windowManager.DrawAll();

// Show instructions
ShowInstructions();

// Start keyboard handling (this will run until Escape is pressed)
await windowManager.HandleKeyboardAsync();

// Cleanup
Console.Clear();
Console.WriteLine("Demo completed! Thanks for trying the ConWin Framework.");

static void CreateDemoWindows(WindowManager windowManager)
{
    // Create a text editor window
    var textEditor = new TextEditorWindow(
        new Position(5, 3), 
        new Size(40, 12), 
        "Text Editor")
    {
        BackgroundColor = ConsoleColor.DarkBlue,
        ForegroundColor = ConsoleColor.White
    };

    // Create a counter window
    var counter = new CounterWindow(
        new Position(50, 3), 
        new Size(25, 8), 
        "Counter")
    {
        BackgroundColor = ConsoleColor.DarkGreen,
        ForegroundColor = ConsoleColor.Yellow
    };

    // Create a log window
    var logger = new LogWindow(
        new Position(5, 18), 
        new Size(70, 8), 
        "Event Log")
    {
        BackgroundColor = ConsoleColor.DarkRed,
        ForegroundColor = ConsoleColor.White
    };

    windowManager.AddWindow(textEditor);
    windowManager.AddWindow(counter);
    windowManager.AddWindow(logger);
}

static void ShowInstructions()
{
    Console.SetCursorPosition(0, 0);
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("ConWin Framework Demo - Use Tab/Shift+Tab to switch focus, Escape to exit");
}

/// <summary>
/// A demo window that allows text input and editing.
/// </summary>
public class TextEditorWindow : Window
{
    private readonly List<string> _lines = new();
    private int _cursorX = 0;
    private int _cursorY = 0;

    public TextEditorWindow(Position position, Size size, string title) 
        : base(position, size, title, BorderStyle.Single)
    {
        _lines.Add("");
    }

    public override void Draw()
    {
        base.Draw();
        
        if (!IsVisible) return;

        // Clear client area and redraw content
        ClearClientArea();
        
        // Draw content
        for (int i = 0; i < Math.Min(_lines.Count, ClientBounds.Size.Height); i++)
        {
            WriteLineAt(0, i, _lines[i]);
        }

        // Draw cursor if focused
        if (HasFocus)
        {
            SetCursorPosition(_cursorX, _cursorY);
            Console.BackgroundColor = ForegroundColor;
            Console.ForegroundColor = BackgroundColor;
            Console.Write(" ");
        }

        // Show instructions
        if (HasFocus)
        {
            WriteAt(0, ClientBounds.Size.Height - 1, "Type to add text, Enter for new line");
        }
    }

    public override bool HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                // Add new line
                if (_cursorY < _lines.Count - 1)
                {
                    _lines.Insert(_cursorY + 1, "");
                }
                else
                {
                    _lines.Add("");
                }
                _cursorY++;
                _cursorX = 0;
                break;

            case ConsoleKey.Backspace:
                // Handle backspace
                if (_cursorX > 0)
                {
                    var line = _lines[_cursorY];
                    _lines[_cursorY] = line.Remove(_cursorX - 1, 1);
                    _cursorX--;
                }
                else if (_cursorY > 0)
                {
                    // Move to end of previous line
                    _cursorX = _lines[_cursorY - 1].Length;
                    _lines[_cursorY - 1] += _lines[_cursorY];
                    _lines.RemoveAt(_cursorY);
                    _cursorY--;
                }
                break;

            case ConsoleKey.LeftArrow:
                if (_cursorX > 0) _cursorX--;
                break;

            case ConsoleKey.RightArrow:
                if (_cursorX < _lines[_cursorY].Length) _cursorX++;
                break;

            case ConsoleKey.UpArrow:
                if (_cursorY > 0)
                {
                    _cursorY--;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                }
                break;

            case ConsoleKey.DownArrow:
                if (_cursorY < _lines.Count - 1)
                {
                    _cursorY++;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                }
                break;

            default:
                // Add character if it's printable
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    var line = _lines[_cursorY];
                    _lines[_cursorY] = line.Insert(_cursorX, keyInfo.KeyChar.ToString());
                    _cursorX++;
                }
                break;
        }

        // Ensure cursor stays within bounds
        _cursorY = Math.Max(0, Math.Min(_cursorY, _lines.Count - 1));
        _cursorX = Math.Max(0, Math.Min(_cursorX, _lines[_cursorY].Length));

        return true;
    }
}

/// <summary>
/// A demo window that shows a counter and responds to key presses.
/// </summary>
public class CounterWindow : Window
{
    private int _counter = 0;

    public CounterWindow(Position position, Size size, string title) 
        : base(position, size, title, BorderStyle.Double)
    {
    }

    public override void Draw()
    {
        base.Draw();
        
        if (!IsVisible) return;

        ClearClientArea();
        
        WriteLineAt(0, 0, $"Count: {_counter}");
        WriteLineAt(0, 2, "Press:");
        WriteLineAt(0, 3, "+ to increment");
        WriteLineAt(0, 4, "- to decrement");
        WriteLineAt(0, 5, "R to reset");
    }

    public override bool HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Add:
            case ConsoleKey.OemPlus:
                _counter++;
                return true;

            case ConsoleKey.Subtract:
            case ConsoleKey.OemMinus:
                _counter--;
                return true;

            case ConsoleKey.R:
                _counter = 0;
                return true;

            default:
                return false;
        }
    }
}

/// <summary>
/// A demo window that logs events and key presses.
/// </summary>
public class LogWindow : Window
{
    private readonly List<string> _logEntries = new();
    private int _scrollOffset = 0;

    public LogWindow(Position position, Size size, string title) 
        : base(position, size, title, BorderStyle.Single)
    {
        AddLogEntry("Log window initialized");
    }

    public override void Draw()
    {
        base.Draw();
        
        if (!IsVisible) return;

        ClearClientArea();
        
        // Show log entries
        int visibleLines = ClientBounds.Size.Height - 1; // Reserve last line for instructions
        int startIndex = Math.Max(0, _logEntries.Count - visibleLines - _scrollOffset);
        
        for (int i = 0; i < Math.Min(visibleLines, _logEntries.Count); i++)
        {
            int entryIndex = startIndex + i;
            if (entryIndex >= 0 && entryIndex < _logEntries.Count)
            {
                WriteLineAt(0, i, _logEntries[entryIndex]);
            }
        }

        // Show instructions
        if (HasFocus)
        {
            WriteAt(0, ClientBounds.Size.Height - 1, "L to add log entry, C to clear");
        }
    }

    public override bool HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.L:
                AddLogEntry($"Manual log entry at {DateTime.Now:HH:mm:ss}");
                return true;

            case ConsoleKey.C:
                _logEntries.Clear();
                AddLogEntry("Log cleared");
                return true;

            case ConsoleKey.PageUp:
                _scrollOffset = Math.Min(_scrollOffset + 1, Math.Max(0, _logEntries.Count - ClientBounds.Size.Height + 1));
                return true;

            case ConsoleKey.PageDown:
                _scrollOffset = Math.Max(_scrollOffset - 1, 0);
                return true;

            default:
                AddLogEntry($"Key pressed: {keyInfo.Key}");
                return true;
        }
    }

    private void AddLogEntry(string message)
    {
        _logEntries.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        
        // Keep only the last 100 entries
        if (_logEntries.Count > 100)
        {
            _logEntries.RemoveAt(0);
        }
    }
}

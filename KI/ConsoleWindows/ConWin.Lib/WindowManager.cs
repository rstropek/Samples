namespace ConWin.Lib;

/// <summary>
/// Manages a collection of windows and handles their drawing order and visibility.
/// </summary>
public class WindowManager
{
    private readonly List<WindowBase> _windows = new();
    private WindowBase? _focusedWindow;

    /// <summary>
    /// Gets the collection of all managed windows.
    /// </summary>
    public IReadOnlyList<WindowBase> Windows => _windows.AsReadOnly();

    /// <summary>
    /// Gets or sets the currently focused window.
    /// </summary>
    public WindowBase? FocusedWindow
    {
        get => _focusedWindow;
        set
        {
            if (_focusedWindow == value) return;

            // Remove focus from the previous window
            if (_focusedWindow != null)
                _focusedWindow.HasFocus = false;

            _focusedWindow = value;

            // Set focus on the new window
            if (_focusedWindow != null)
                _focusedWindow.HasFocus = true;
        }
    }

    /// <summary>
    /// Adds a window to the manager.
    /// </summary>
    /// <param name="window">The window to add.</param>
    public void AddWindow(WindowBase window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        if (!_windows.Contains(window))
        {
            _windows.Add(window);
            
            // If this is the first window, give it focus
            if (_focusedWindow == null)
                FocusedWindow = window;
        }
    }

    /// <summary>
    /// Removes a window from the manager.
    /// </summary>
    /// <param name="window">The window to remove.</param>
    /// <returns>True if the window was removed; otherwise, false.</returns>
    public bool RemoveWindow(WindowBase window)
    {
        if (window == null)
            return false;

        bool removed = _windows.Remove(window);
        
        if (removed && _focusedWindow == window)
        {
            // If the focused window was removed, focus the next available window
            FocusedWindow = _windows.FirstOrDefault();
        }

        return removed;
    }

    /// <summary>
    /// Clears all windows from the manager.
    /// </summary>
    public void Clear()
    {
        _windows.Clear();
        FocusedWindow = null;
    }

    /// <summary>
    /// Sets focus to the specified window.
    /// </summary>
    /// <param name="window">The window to focus.</param>
    /// <returns>True if the window was focused; otherwise, false.</returns>
    public bool SetFocus(WindowBase window)
    {
        if (window == null || !_windows.Contains(window))
            return false;

        FocusedWindow = window;
        return true;
    }

    /// <summary>
    /// Cycles focus to the next window in the collection.
    /// </summary>
    public void FocusNext()
    {
        if (_windows.Count == 0) return;

        if (_focusedWindow == null)
        {
            FocusedWindow = _windows.First();
            return;
        }

        int currentIndex = _windows.IndexOf(_focusedWindow);
        int nextIndex = (currentIndex + 1) % _windows.Count;
        FocusedWindow = _windows[nextIndex];
    }

    /// <summary>
    /// Cycles focus to the previous window in the collection.
    /// </summary>
    public void FocusPrevious()
    {
        if (_windows.Count == 0) return;

        if (_focusedWindow == null)
        {
            FocusedWindow = _windows.Last();
            return;
        }

        int currentIndex = _windows.IndexOf(_focusedWindow);
        int previousIndex = currentIndex == 0 ? _windows.Count - 1 : currentIndex - 1;
        FocusedWindow = _windows[previousIndex];
    }

    /// <summary>
    /// Starts an async keyboard handling loop that continues until Escape is pressed.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to stop the loop.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task HandleKeyboardAsync(CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    
                    // Check for Escape to exit
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    // Handle built-in navigation keys
                    if (HandleNavigationKeys(keyInfo))
                    {
                        continue; // Navigation keys already redraw
                    }

                    // Pass the key to the focused window
                    bool keyHandled = false;
                    if (_focusedWindow != null && _focusedWindow.IsVisible)
                    {
                        keyHandled = _focusedWindow.HandleKeyPress(keyInfo);
                    }

                    // Redraw all windows if the key was handled
                    if (keyHandled)
                    {
                        DrawAll();
                    }
                }
                else
                {
                    // Small delay to prevent busy waiting
                    Thread.Sleep(10);
                }
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Handles built-in navigation keys (Tab, Shift+Tab).
    /// </summary>
    /// <param name="keyInfo">The key information.</param>
    /// <returns>True if the key was handled; otherwise, false.</returns>
    private bool HandleNavigationKeys(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Tab when keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift):
                FocusPrevious();
                DrawAll(); // Redraw to show focus changes
                return true;
            
            case ConsoleKey.Tab:
                FocusNext();
                DrawAll(); // Redraw to show focus changes
                return true;
            
            default:
                return false;
        }
    }

    /// <summary>
    /// Draws all managed windows in the correct order, respecting z-index and parent-child relationships.
    /// </summary>
    public void DrawAll()
    {
        // Clear the console
        Console.Clear();

        // Calculate visibility for all windows
        CalculateVisibility();

        // Get the drawing order: top-level windows first, then their children recursively
        var drawingOrder = GetDrawingOrder();

        // Draw each window in order
        foreach (var window in drawingOrder)
        {
            if (window.IsVisible)
            {
                window.Draw();
            }
        }

        // Draw focus indicator for the focused window
        DrawFocusIndicator();
    }

    /// <summary>
    /// Draws a focus indicator for the currently focused window.
    /// </summary>
    private void DrawFocusIndicator()
    {
        if (_focusedWindow == null || !_focusedWindow.IsVisible)
            return;

        // Save current console colors
        var originalBg = Console.BackgroundColor;
        var originalFg = Console.ForegroundColor;

        try
        {
            // Draw a simple focus indicator in the bottom-right corner of the focused window
            var bounds = _focusedWindow.Bounds;
            if (bounds.Size.Width > 0 && bounds.Size.Height > 0)
            {
                Console.SetCursorPosition(bounds.Position.X + bounds.Size.Width - 1, bounds.Position.Y + bounds.Size.Height - 1);
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("*");
            }
        }
        finally
        {
            // Restore console colors
            Console.BackgroundColor = originalBg;
            Console.ForegroundColor = originalFg;
        }
    }

    /// <summary>
    /// Calculates the visibility of all windows based on occlusion by other windows.
    /// </summary>
    private void CalculateVisibility()
    {
        // Get all top-level windows (windows without parents) sorted by z-index
        var topLevelWindows = _windows
            .Where(w => w.Parent == null)
            .OrderBy(w => w.ZIndex)
            .ToList();

        // Reset visibility for all windows
        foreach (var window in _windows)
        {
            window.IsVisible = true;
        }

        // Check each window against windows that should be drawn after it
        for (int i = 0; i < topLevelWindows.Count; i++)
        {
            var currentWindow = topLevelWindows[i];
            
            // Check if this window is completely obscured by any window drawn after it
            for (int j = i + 1; j < topLevelWindows.Count; j++)
            {
                var laterWindow = topLevelWindows[j];
                
                if (laterWindow.Bounds.Contains(currentWindow.Bounds))
                {
                    currentWindow.IsVisible = false;
                    break;
                }
            }

            // If the parent window is not visible, children should also not be visible
            if (!currentWindow.IsVisible)
            {
                SetChildrenVisibility(currentWindow, false);
            }
        }
    }

    /// <summary>
    /// Recursively sets the visibility of all child windows.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <param name="isVisible">The visibility to set.</param>
    private void SetChildrenVisibility(WindowBase parent, bool isVisible)
    {
        foreach (var child in parent.Children)
        {
            child.IsVisible = isVisible;
            SetChildrenVisibility(child, isVisible);
        }
    }

    /// <summary>
    /// Gets the drawing order for all windows, respecting z-index and parent-child relationships.
    /// </summary>
    /// <returns>An ordered list of windows to draw.</returns>
    private List<WindowBase> GetDrawingOrder()
    {
        var drawingOrder = new List<WindowBase>();

        // Get all top-level windows sorted by z-index
        var topLevelWindows = _windows
            .Where(w => w.Parent == null)
            .OrderBy(w => w.ZIndex)
            .ToList();

        // Add each top-level window and its children recursively
        foreach (var window in topLevelWindows)
        {
            AddWindowAndChildren(window, drawingOrder);
        }

        return drawingOrder;
    }

    /// <summary>
    /// Recursively adds a window and all its children to the drawing order.
    /// </summary>
    /// <param name="window">The window to add.</param>
    /// <param name="drawingOrder">The list to add windows to.</param>
    private void AddWindowAndChildren(WindowBase window, List<WindowBase> drawingOrder)
    {
        // Add the parent window first
        drawingOrder.Add(window);

        // Then add all children, sorted by their z-index
        var sortedChildren = window.Children.OrderBy(c => c.ZIndex).ToList();
        foreach (var child in sortedChildren)
        {
            AddWindowAndChildren(child, drawingOrder);
        }
    }

    /// <summary>
    /// Brings a window to the front by setting its z-index higher than all other windows at the same level.
    /// </summary>
    /// <param name="window">The window to bring to front.</param>
    public void BringToFront(WindowBase window)
    {
        if (window == null || !_windows.Contains(window))
            return;

        // Get all windows at the same level (same parent)
        var siblings = window.Parent?.Children ?? _windows.Where(w => w.Parent == null);
        
        // Find the highest z-index among siblings
        int maxZIndex = siblings.Where(w => w != window).Select(w => w.ZIndex).DefaultIfEmpty(-1).Max();
        
        // Set this window's z-index to be higher
        window.ZIndex = maxZIndex + 1;
    }

    /// <summary>
    /// Sends a window to the back by setting its z-index lower than all other windows at the same level.
    /// </summary>
    /// <param name="window">The window to send to back.</param>
    public void SendToBack(WindowBase window)
    {
        if (window == null || !_windows.Contains(window))
            return;

        // Get all windows at the same level (same parent)
        var siblings = window.Parent?.Children ?? _windows.Where(w => w.Parent == null);
        
        // Find the lowest z-index among siblings
        int minZIndex = siblings.Where(w => w != window).Select(w => w.ZIndex).DefaultIfEmpty(1).Min();
        
        // Set this window's z-index to be lower
        window.ZIndex = minZIndex - 1;
    }
} 
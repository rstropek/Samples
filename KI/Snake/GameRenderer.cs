namespace Snake.Game;

public class GameUI
{
    public Task StartKeyboardListener(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            do
            {
                // We wait for a key press because the task must be cancellable
                while (!Console.KeyAvailable && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(10, cancellationToken);
                    }
                    catch (TaskCanceledException) {}
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    var key = Console.ReadKey(true);
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        KeyPressed?.Invoke(this, key.Key);
                    }
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        });
    }

    private void DrawLine(int x1, int y1, int x2, int y2)
    {
        if (x1 != x2 && y1 != y2)
        {
            throw new ArgumentException("Only horizontal or vertical lines are supported");
        }

        Console.BackgroundColor = ConsoleColor.White;
        if (x1 == x2) // Vertical line
        {
            int start = Math.Min(y1, y2);
            int length = Math.Abs(y2 - y1) + 1;
            for (int i = 0; i < length; i++)
            {
                Console.SetCursorPosition(x1, start + i);
                Console.Write(' ');
            }
        }
        else // Horizontal line
        {
            int start = Math.Min(x1, x2);
            int length = Math.Abs(x2 - x1) + 1;
            Console.SetCursorPosition(start, y1);
            Console.Write(new string(' ', length));
        }
    }

    public void DrawRectangle(int x, int y, int width, int height)
    {
        DrawLine(x, y, x + width - 1, y);                    // Top
        DrawLine(x, y + height - 1, x + width - 1, y + height - 1);  // Bottom
        DrawLine(x, y, x, y + height - 1);                    // Left
        DrawLine(x + width - 1, y, x + width - 1, y + height - 1);   // Right
    }

    public void ClearScreen()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
    }

    public void Render(Game game, int leftMargin, int topMargin)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(game.Apple.X + leftMargin, game.Apple.Y + topMargin);
        Console.Write(' ');

        Console.BackgroundColor = ConsoleColor.DarkGreen;
        foreach (var pos in game.SnakeBody)
        {
            Console.SetCursorPosition(pos.X + leftMargin, pos.Y + topMargin);
            Console.Write(' ');
            Console.BackgroundColor = ConsoleColor.Green;
        }

        Console.ResetColor();
    }

    public void DrawText(int x, int y, string text)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x, y);
        Console.Write(text);
        Console.ResetColor();
    }

    public event EventHandler<ConsoleKey>? KeyPressed;
}
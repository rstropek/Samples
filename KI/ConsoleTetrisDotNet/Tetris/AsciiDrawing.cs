namespace ConsoleTetris;

public enum DrawMode
{
    Normal,
    Remove
}

public class AsciiDrawing(ITetrisConsole console, Blocks blocks)
{
    private const char VerticalBar = '\u25AE';

    public void DrawLine(int x1, int y1, int x2, int y2)
    {
        if (x1 != x2 && y1 != y2)
        {
            throw new ArgumentException("Only vertical and horizontal lines are supported");
        }

        if (x1 == x2)
        {
            // Vertical line
            int start = Math.Min(y1, y2);
            int end = Math.Max(y1, y2);
            for (var y = start; y <= end; y++)
            {
                console.SetCursorPosition(x1, y);
                console.Write(AsciiDrawing.VerticalBar);
            }
        }
        else
        {
            // Horizontal line
            int start = Math.Min(x1, x2);
            int end = Math.Max(x1, x2);
            console.SetCursorPosition(start, y1);
            console.Write(new string(AsciiDrawing.VerticalBar, end - start + 1));
        }
    }

    public void DrawBlock(int x, int y, string block, DrawMode mode = DrawMode.Normal)
    {
        var blockSpan = block.AsSpan();

        blocks.Iterate(x, y, block, (x, y, c) =>
        {
            if (!char.IsWhiteSpace(c) || mode == DrawMode.Remove)
            {
                console.SetCursorPosition(x, y);
                console.Write(mode == DrawMode.Remove ? ' ' : c);
            }
        });
    }

    public void DrawRectangle(int x, int y, int width, int height)
    {
        // Draw top horizontal line
        DrawLine(x, y, x + width + 1, y);

        // Draw bottom horizontal line
        DrawLine(x, y + height + 1, x + width + 1, y + height + 1);

        // Draw left vertical line
        DrawLine(x, y, x, y + height + 1);

        // Draw right vertical line
        DrawLine(x + width + 1, y, x + width + 1, y + height + 1);
    }
}

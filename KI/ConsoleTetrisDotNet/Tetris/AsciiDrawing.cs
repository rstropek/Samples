namespace ConsoleTetris;

public class AsciiDrawing(ITetrisConsole console, TetrisBlock blocks)
{
    private const char VerticalBar = '*';

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

    public void DrawBlock(int x, int y, string block)
    {
        var blockSpan = block.AsSpan();

        blocks.IterateOverCharsInBlock(x, y, block, (x, y, c) =>
        {
            if (!char.IsWhiteSpace(c))
            {
                console.SetCursorPosition(x, y);
                console.Write(c);
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

    public void ClearRectangle(int width, int height)
    {
        var emptyLine = new string(' ', width);
        for (var y = 0; y < height; y++)
        {
            console.SetCursorPosition(0, y);
            console.Write(emptyLine);
        }
    }
}

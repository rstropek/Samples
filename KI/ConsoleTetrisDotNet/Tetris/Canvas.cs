namespace ConsoleTetris;

class Canvas(int width, int height, ITetrisConsole console, TetrisBlock blocks)
{
    private readonly bool[,] pixels = new bool[width, height];

    public void SetPixel(int x, int y, string block)
    {
        blocks.IterateOverCharsInBlock(x, y, block, (col, line, c) =>
        {
            if (!char.IsWhiteSpace(c))
            {
                pixels[col, line] = true;
            }
        });
    }

    public void Draw()
    {
        for (var y = 0; y < height; y++)
        {
            console.SetCursorPosition(0, y);
            for (var x = 0; x < width; x++)
            {
                console.Write(pixels[x, y] ? '#' : ' ');
            }
        }
    }

    public int Shift()
    {
        bool IsLineFull(int y)
        {
            for (var x = 0; x < width; x++)
            {
                if (!pixels[x, y])
                {
                    return false;
                }
            }
            return true;
        }

        var removed = 0;
        for (var y = height - 1; y >= 0; )
        {
            if (IsLineFull(y))
            {
                for (var line = y; line > 0; line--)
                {
                    for (var x = 0; x < width; x++)
                    {
                        pixels[x, line] = pixels[x, line - 1];
                    }
                }
                for (var x = 0; x < width; x++)
                {
                    pixels[x, 0] = false;
                }

                removed++;
            }
            else
            {
                y--;
            }
        }

        return removed;
    }

    public bool Fits(int x, int y, string block)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        var doesFit = true;
        blocks.IterateOverCharsInBlock(x, y, block, (col, line, c) =>
        {
            if (col >= width || line >= height || (pixels[col, line] && !char.IsWhiteSpace(c)))
            {
                doesFit = false;
            }
        });

        return doesFit;
    }

    public int Drop(int x, int y, string block)
    {
        for (; Fits(x, y + 1, block); y++) ;
        return y;
    }
}
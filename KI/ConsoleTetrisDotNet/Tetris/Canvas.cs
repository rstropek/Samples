namespace ConsoleTetris;

class Canvas(int width, int height, ITetrisConsole console, Blocks blocks)
{
    private readonly bool[,] pixels = new bool[width, height];

    public void SetPixel(int x, int y, string block)
    {
        blocks.Iterate(x, y, block, (col, line, c) =>
        {
            if (!char.IsWhiteSpace(c))
            {
                pixels[col, line] = true;
                console.SetCursorPosition(col, line);
                console.Write(c);
            }
        });
    }

    public void Draw(DrawMode mode)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                console.SetCursorPosition(x, y);
                if (pixels[x, y])
                {
                    console.Write(mode == DrawMode.Normal ? '#' : ' ');
                }
                else
                {
                    console.Write(' ');
                }
            }
        }
    }

    public void Shift()
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

        var anyLinesFull = false;
        for (var y = height - 1; y >= 0; y--)
        {
            if (IsLineFull(y))
            {
                anyLinesFull = true;
            }
        }

        if (anyLinesFull)
        {
            Draw(DrawMode.Remove);

            for (var y = height - 1; y >= 0; y--)
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

                    y++;
                }
            }

            Draw(DrawMode.Normal);
        }
    }

    public bool Fits(int x, int y, string block)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        var doesFit = true;
        blocks.Iterate(x, y, block, (col, line, c) =>
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
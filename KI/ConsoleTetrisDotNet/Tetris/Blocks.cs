namespace ConsoleTetris;

public class Blocks
{
    public static readonly string[] BlockLayouts =
    [
        """
        ##
        ##
        """,
        """
        #
        #
        #
        #
        """,
        """
         # 
        ###
        """,
        """
          #
        ###
        """,
        """
        #  
        ###
        """,
        """
        ## 
         ##
        """,
        """
         ##
        ## 
        """,
    ];

    public string GetRandomBlock() => Random.Shared.GetItems(BlockLayouts, 1)[0];

    public string Rotate(string block)
    {
        var (width, height) = GetBlockDimensions(block);

        return string.Create(width * height + (width - 1), block, (chars, originalBlock) =>
        {
            var rotatedIndex = 0;
            for (var x = 0; x < width; x++)
            {
                for (var y = height - 1; y >= 0; y--)
                {
                    var index = y * (width + 1) + x;
                    // Check if the index is within bounds
                    if (index < originalBlock.Length)
                    {
                        chars[rotatedIndex++] = originalBlock[index];
                    }
                    else
                    {
                        // If out of bounds, add a space or another placeholder
                        chars[rotatedIndex++] = ' ';
                    }
                }
                if (x < width - 1)
                {
                    chars[rotatedIndex++] = '\n';
                }
            }
        });
    }

    public (int width, int height) GetBlockDimensions(string block)
    {
        var width = 0;
        var height = 1;

        for (var i = 0; i < block.Length; i++)
        {
            if (block[i] == '\n')
            {
                height++;
                if (width == 0)
                {
                    width = i;
                }
            }
        }

        if (width == 0)
        {
            width = block.Length;
        }

        return (width, height);
    }

    public void Iterate(int x, int y, string block, Action<int, int, char> action)
    {
        for (int line = y, col = x, ix = 0; ix < block.Length; ix++, col++)
        {
            if (block[ix] != '\n')
            {
                action(col, line, block[ix]);
            }
            else
            {
                line++;
                col = x - 1;
            }
        }
    }
}
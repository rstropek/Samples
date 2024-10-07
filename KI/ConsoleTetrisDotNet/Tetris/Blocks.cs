[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tetris.Tests")]

namespace ConsoleTetris;

public static class TetrisBlockImpl
{
    internal static readonly string[] BlockLayouts =
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

    internal static string GetRandomBlockImpl() => Random.Shared.GetItems(BlockLayouts, 1)[0];

    internal static string RotateImpl(string block)
    {
        var (width, height) = TetrisBlockImpl.GetBlockDimensionsImpl(block);

        return string.Create(width * height + (width - 1), block, (chars, originalBlock) =>
        {
            var rotatedIndex = 0;

            // Iterate through each column of the original block
            for (var x = 0; x < width; x++)
            {
                // For each column, iterate through the rows from bottom to top
                // This effectively rotates the block 90 degrees clockwise
                for (var y = height - 1; y >= 0; y--)
                {
                    // Calculate the index in the original block
                    // This maps (x,y) coordinates to a linear index in the string
                    var index = y * (width + 1) + x;
                    
                    // Check if the calculated index is within the original block's bounds
                    if (index < originalBlock.Length)
                    {
                        // Copy the character from the original block to the rotated position
                        chars[rotatedIndex++] = originalBlock[index];
                    }
                    else
                    {
                        // If the index is out of bounds (due to irregular shapes),
                        // fill with a space to maintain the block's rectangular form
                        chars[rotatedIndex++] = ' ';
                    }
                }
                // Add a newline after each column (now row) except for the last one
                if (x < width - 1)
                {
                    chars[rotatedIndex++] = '\n';
                }
            }
        });
    }

    public static (int width, int height) GetBlockDimensionsImpl(string block)
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

    public static void IterateOverCharsInBlockImpl(int x, int y, string block, Action<int, int, char> action)
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

#pragma warning disable CA1822 // Mark members as static
public class TetrisBlock
{
    public string GetRandomBlock() => TetrisBlockImpl.GetRandomBlockImpl();

    public string Rotate(string block) => TetrisBlockImpl.RotateImpl(block);

    public (int width, int height) GetBlockDimensions(string block) => TetrisBlockImpl.GetBlockDimensionsImpl(block);
                
    public void IterateOverCharsInBlock(int x, int y, string block, Action<int, int, char> action) =>
        TetrisBlockImpl.IterateOverCharsInBlockImpl(x, y, block, action);
}
#pragma warning restore CA1822 // Mark members as static

public static class StringExtensions
{
    public static string Rotate(this string block) => TetrisBlockImpl.RotateImpl(block);

    public static (int width, int height) GetBlockDimensions(this string block) => 
        TetrisBlockImpl.GetBlockDimensionsImpl(block);

    public static void IterateOverCharsInBlock(this string block, int x, int y, Action<int, int, char> action) =>
        TetrisBlockImpl.IterateOverCharsInBlockImpl(x, y, block, action);
}
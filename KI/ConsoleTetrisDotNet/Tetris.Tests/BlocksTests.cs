namespace Tetris.Tests;

using Xunit;
using ConsoleTetris;

public class BlocksTests
{
    [Theory]
    [InlineData(0, 2, 2)] // Square block
    [InlineData(1, 1, 4)] // Line block
    [InlineData(2, 3, 2)] // T block
    [InlineData(3, 3, 2)] // Reverse L block
    [InlineData(4, 3, 2)] // L block
    [InlineData(5, 3, 2)] // S block
    [InlineData(6, 3, 2)] // Z block
    public void GetBlockDimensions_ReturnsCorrectDimensions(int blockIndex, int expectedWidth, int expectedHeight)
    {
        var block = Blocks.BlockLayouts[blockIndex];
        var (width, height) = new Blocks().GetBlockDimensions(block);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedHeight, height);
    }

    [Theory]
    [MemberData(nameof(BlockLayoutsData))]
    public void RotateFourTimes_ShouldReturnOriginalBlock(string block)
    {
        // Arrange
        var blocks = new Blocks();

        // Act
        var rotated = block;
        for (int i = 0; i < 4; i++)
        {
            rotated = blocks.Rotate(rotated);
        }

        // Assert
        Assert.Equal(block, rotated);
    }

    public static TheoryData<string> BlockLayoutsData
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var layout in Blocks.BlockLayouts)
            {
                data.Add(layout);
            }
            return data;
        }
    }
}

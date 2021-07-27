namespace TicTacToe.Tests
{
    public class BoardContentTests
    {
        [Fact]
        public void FactoryCreatesContent() => Assert.Equal(9, new BoardContent(null).Content.Length);

        [Fact]
        public void FactoryFillsContent()
        {
            Assert.Equal(9, new BoardContent(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }).Content.Length);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetRow(int index) => Assert.Equal(3, new BoardContent(null).GetRow(index).Length);


        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void GetRowInvalidIndex(int index)
            => Assert.Throws<ArgumentOutOfRangeException>(() => new BoardContent(null).GetRow(index));

        [Theory]
        [MemberData(nameof(Squares))]
        public void CalculateIndex(int expectedIx, int col, int row)
            => Assert.Equal(expectedIx, BoardContent.CalculateIndex(col, row));

        public static IEnumerable<object[]> Squares
            => Enumerable.Range(0, 2).SelectMany(
                col => Enumerable.Range(0, 2).Select(
                    row => new object[] { row * 3 + col, col, row })).ToArray();

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(3, 0)]
        [InlineData(0, 3)]
        public void CalculateIndexInvalidIndex(int col, int row)
            => Assert.Throws<ArgumentOutOfRangeException>(() => BoardContent.CalculateIndex(col, row));

        [Fact]
        public void IndexerGetSuccess()
        {
            _ = new BoardContent(null).Get(0, 0);
            _ = BoardContentFactory.Create()[(0, 0)];
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void IndexerSetSuccess(int col, int row)
        {
            var content = BoardContentFactory.Create();
            content = content.Set(SquareContent.X, col, row);
            Assert.Equal(SquareContent.X, content.Get(col, row));

            var expected = new byte[3 * 3];
            expected[BoardContent.CalculateIndex(col, row)] = SquareContent.X;
            Assert.True(content.Content.SequenceEqual(expected));
        }

        [Fact]
        public void IndexerSetInvalidValue()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new BoardContent(null).Set(99, 1, 1));

        [Fact]
        public void ContentEquals() => Assert.True(new BoardContent(null).Equals(new BoardContent(null)));

        [Fact]
        public void ContentObjectEquals() => Assert.True(new BoardContent(null).Equals((object)new BoardContent(null)));

        [Fact]
        public void ContentObjectNotEquals() => Assert.False(new BoardContent(null).Equals(string.Empty));

        [Fact]
        public void ContentHashcodeEquals()
            => Assert.Equal(new BoardContent(null).GetHashCode(), new BoardContent(null).GetHashCode());

        [Fact]
        public void ContentEqualsOp() => Assert.True(new BoardContent(null) == new BoardContent(null));

        [Fact]
        public void ContentNotEqualsOp() => Assert.False(new BoardContent(null) != new BoardContent(null));
    }
}

namespace TicTacToe.Tests
{
    public class BoardContentTests
    {
        /// <summary>
        /// Ensures that <see cref="BoardContentFactory"/> creates board without exception.
        /// </summary>
        [Fact]
        public void FactoryCreatesContent() => BoardContentFactory.Create();

        [Fact]
        public void ConstructorFillsEmptyBoard()
        {
            var content = new BoardContent(null).content;
            Assert.Equal(9, content.Length);
            Assert.All(content, c => Assert.Equal(SquareContent.EMPTY, c));
        }

        [Fact]
        public void ConstructorCopiesContent()
        {
            var content = new BoardContent(new byte[] { SquareContent.X, 0, 0, 0, 0, 0, 0, 0, 0 }).content;
            Assert.Equal(SquareContent.X, content[0]);
            Assert.All(content[1..], c => Assert.Equal(SquareContent.EMPTY, c));
        }

        /// <summary>
        /// Ensure that the <see cref="BoardContent.Content"/> property copies internal value.
        /// </summary>
        [Fact]
        public void ContentPropertyCopies()
        {
            var content = new BoardContent(null);
            Assert.NotSame(content.Content, content.content);
        }

        /// <summary>
        /// Ensure that we can ask for all three rows
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetRow(int index) => Assert.Equal(3, new BoardContent(null).GetRow(index).Length);

        /// <summary>
        /// Ensure proper exception in case of invalid row index
        /// </summary>
        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void GetRowInvalidIndex(int index)
            => Assert.Throws<ArgumentOutOfRangeException>("row", () => new BoardContent(null).GetRow(index));

        [Theory]
        [MemberData(nameof(Squares))]
        public void CalculateIndexHandlesColRowCorrectly(int expectedIx, int col, int row)
            => Assert.Equal(expectedIx, BoardContent.CalculateIndex(col, row));

        public static IEnumerable<object[]> Squares()
        {
            var ix = 0;
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    yield return new object[] { ix++, col, row };
                }
            }
        }

        /// <summary>
        /// Ensures proper exceptions in case of invalid column/row indexes
        /// </summary>
        [Theory]
        [InlineData(-1, 0, "col")]
        [InlineData(0, -1, "row")]
        [InlineData(3, 0, "col")]
        [InlineData(0, 3, "row")]
        public void CalculateIndexInvalidIndex(int col, int row, string argument)
            => Assert.Throws<ArgumentOutOfRangeException>(argument, () => BoardContent.CalculateIndex(col, row));

        /// <summary>
        /// Ensure that we can access a square using <see cref="BoardContent.Get(int, int)"/> without exception.
        /// </summary>
        [Fact]
        public void GetSquareContent() => _ = new BoardContent(null).Get(0, 0);

        /// <summary>
        /// Ensure that we can access a square using the indexer without exception.
        /// </summary>
        [Fact]
        public void Indexer() => _ = BoardContentFactory.Create()[(0, 0)];

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
            Assert.Equal(expected, content.Content);
        }

        [Fact]
        public void SetInvalidValue()
            => Assert.Throws<ArgumentOutOfRangeException>("value", () => new BoardContent(null).Set(99, 1, 1));

        [Fact]
        public void HasEmptySquares()
            => Assert.True(BoardContentFactory.Create().HasEmptySquares);

        [Fact]
        public void HasNoEmptySquares()
            => Assert.False(new BoardContent(Enumerable.Range(0, 9).Select(_ => SquareContent.X).ToArray()).HasEmptySquares);

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

namespace TicTacToe.Tests
{
    public class GameTests : IClassFixture<TestBoardFixture>
    {
        private readonly TestBoardFixture fixture;

        public GameTests(TestBoardFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void IsValid() => Assert.True(Game.IsValid(fixture.TestBoards.Valid));

        [Fact]
        public void IsInvalid1() => Assert.False(Game.IsValid(fixture.TestBoards.Invalid1));

        [Fact]
        public void IsInvalid2() => Assert.False(Game.IsValid(fixture.TestBoards.Invalid2));

        [Fact]
        public void WinnerRow()
        {
            var contentMock = new Mock<IBoardContent>();
            contentMock.Setup(c => c.Get(It.IsAny<int>(), 1)).Returns(1).Verifiable();

            var game = new Game("Foo", "Bar", contentMock.Object);
            Assert.Equal("Foo", game.GetWinner());
            Assert.Equal("Foo", game.GetWinnerFromRows());
            Assert.Null(game.GetWinnerFromColumns());
            Assert.Null(game.GetWinnerFromDiagonals());
            contentMock.VerifyAll();
        }

        [Fact]
        public void WinnerColumn()
        {
            var contentMock = new Mock<IBoardContent>();
            contentMock.Setup(c => c.Get(1, It.IsAny<int>())).Returns(2).Verifiable();

            var game = new Game("Foo", "Bar", contentMock.Object);
            Assert.Equal("Bar", game.GetWinner());
            Assert.Null(game.GetWinnerFromRows());
            Assert.Equal("Bar", game.GetWinnerFromColumns());
            Assert.Null(game.GetWinnerFromDiagonals());
            contentMock.VerifyAll();
        }

        [Fact]
        public void WinnerDiagonal()
        {
            var contentMock = new Mock<IBoardContent>();
            contentMock.Setup(c => c.Get(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((col, row) => col == row ? (byte)1 : (byte)0);

            var game = new Game("Foo", "Bar", contentMock.Object);
            Assert.Equal("Foo", game.GetWinner());
            Assert.Null(game.GetWinnerFromRows());
            Assert.Null(game.GetWinnerFromColumns());
            Assert.Equal("Foo", game.GetWinnerFromDiagonals());
            contentMock.VerifyAll();
        }

        [Fact]
        public void WinnerCallCount()
        {
            var contentMock = new Mock<IBoardContent>();

            var game = new Game("Foo", "Bar", contentMock.Object);
            Assert.Null(game.GetWinner());

            contentMock.Verify(c => c.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(7));
        }

        [Fact]
        public void WhoIsNext()
        {
            var contentMock = new Mock<IBoardContent>();
            contentMock.SetupGet(c => c.Content).Returns(new byte[3 * 3]);
            contentMock.Setup(c => c.Set(It.IsAny<byte>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new BoardContent(new byte[3 * 3]));

            var game = new Game("Foo", "Bar", contentMock.Object);
            Assert.Equal("Foo", game.WhoIsNext);
            game.Set(0, 0);
            Assert.Equal("Bar", game.WhoIsNext);
        }
    }
}

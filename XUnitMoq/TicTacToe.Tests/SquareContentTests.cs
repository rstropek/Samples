namespace TicTacToe.Tests
{
    public class SquareContentTests
    {
        [Fact]
        public void EmptyHasToBeZero() => Assert.Equal(0, SquareContent.EMPTY);
    }
}

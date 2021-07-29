namespace TicTacToe.Tests
{
    public class SquareContentTests
    {
        /// <summary>
        /// Ensure that <see cref="SquareContent.EMPTY"/> has the discriminant value 0.
        /// </summary>
        /// <remarks>
        /// This value is important for the algorithms to work. Unit test ensure that
        /// nobody changes the value of <see cref="SquareContent.EMPTY"/> by accident.
        /// </remarks>
        [Fact]
        public void EmptyHasToBeZero() => Assert.Equal(0, SquareContent.EMPTY);
    }
}

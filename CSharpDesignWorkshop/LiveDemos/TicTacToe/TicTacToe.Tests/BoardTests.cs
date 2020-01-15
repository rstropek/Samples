using System;
using TicTacToe.Logic;
using Xunit;

namespace TicTacToe.Tests
{
    public class BoardTests
    {
        [Fact]
        public void SuccessfullyStartNewGame()
        {
            var board = new Board();
            Assert.NotNull(board);
        }

        [Fact]
        public void ChoosesValidFirstPlayer()
        {
            var board = new Board();
            Assert.True(board.PlayerForNextTurn == 'X' || board.PlayerForNextTurn == 'O');
        }

        [Fact]
        public void ChoosesRandomFirstPlayer()
        {
            var xCount = 0;
            var oCount = 0;
            for (var i = 0; i < 100; i++)
            {
                var board = new Board();
                if (board.PlayerForNextTurn == 'X')
                {
                    xCount++;
                }
                else
                {
                    oCount++;
                }
            }

            Assert.True(xCount > 0 && oCount > 0);
        }

        [Fact]
        public void ApplyValidTurn()
        {
            var board = new Board();
            board.ApplyTurn(0, 0);
        }

        [Fact]
        public void ApplyTurnWithInvalidCoordinates()
        {
            var board = new Board();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => board.ApplyTurn(7, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => board.ApplyTurn(0, 9));
        }

        [Fact]
        public void ApplyTurnWithWrongPlayer()
        {
            var board = new Board();

            Assert.Throws<ArgumentException>(
                () => board.ApplyTurn(
                    board.PlayerForNextTurn == 'X' ? 'O' : 'X', 7, 0));
        }

        [Fact]
        public void ApplyDuplicateTurn()
        {
            Assert.Throws<AlreadyOccupiedException>(() =>
            {
                var board = new Board();
                board.ApplyTurn(0, 0);
                board.ApplyTurn(0, 0);
            });
        }

        [Fact]
        public void TryApplyDuplicateTurn()
        {
            var board = new Board();
            Assert.True(board.TryApplyTurn(0, 0));
            Assert.False(board.TryApplyTurn(0, 0));
        }

        [Fact]
        public void ApplyTurnAfterWin()
        {
            var board = new Board();

            PlayWiningGame(board);
            Assert.Throws<InvalidOperationException>(() => board.ApplyTurn(2, 1));
        }

        private static void PlayWiningGame(Board board)
        {
            board.ApplyTurn(0, 0);
            board.ApplyTurn(0, 1);
            board.ApplyTurn(1, 0);
            board.ApplyTurn(0, 2);
            board.ApplyTurn(2, 0);
        }

        [Fact]
        public void DetectWinner()
        {
            var board = new Board();
            Assert.Null(board.GetWinner());

            var expectedWinner = board.PlayerForNextTurn;
            PlayWiningGame(board);

            Assert.Equal(expectedWinner, board.GetWinner());
        }

        [Fact]
        public void SerializeGame()
        {
            #region SerializeGameExample
            var board = new Board();

            IBoardSerializer serializer = new BoardBase64Serializer();
            var serializedBoard = serializer.Serialize(board);

            var rehydratedBoard = serializer.Deserialize(serializedBoard);
            Assert.Equal(board, rehydratedBoard);
            #endregion
        }

        [Fact]
        public void SerializeInvalidGame()
        {
            Assert.Throws<ArgumentException>(
                () => new BoardBase64Serializer().Deserialize("Dummy"));
        }
    }
}

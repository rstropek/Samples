using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Logic
{
    /// <summary>
    /// Holds the state of a TicTacToe board.
    /// </summary>
    public class Board : IEquatable<Board>
    {
        public char PlayerForNextTurn { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class
        /// </summary>
        /// <remarks>
        /// Game is immediately started.
        /// </remarks>
        public Board()
        {
            var rand = new Random();
            PlayerForNextTurn = rand.Next(0, 2) == 0 ? 'X' : 'O';
        }

        public void ApplyTurn(char player, byte x, byte y)
        {
            throw new NotImplementedException();
        }

        public void ApplyTurn(byte x, byte y) => ApplyTurn(PlayerForNextTurn, x, y);

        public bool TryApplyTurn(int x, int y)
        {
            throw new NotImplementedException();
        }

        public char? GetWinner()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Board other)
        {
            throw new NotImplementedException();
        }
    }
}

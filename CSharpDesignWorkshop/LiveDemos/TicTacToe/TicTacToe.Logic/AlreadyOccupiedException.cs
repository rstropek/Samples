using System;

namespace TicTacToe.Logic
{
    public class AlreadyOccupiedException : Exception
    {
        public AlreadyOccupiedException(string message) : base(message)
        {
        }

        public AlreadyOccupiedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AlreadyOccupiedException()
        {
        }
    }
}
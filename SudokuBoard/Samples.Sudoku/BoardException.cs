namespace Samples.Sudoku
{
    using System;
    using System.Runtime.Serialization;

	/// <summary>
	/// Represents an exception that can happen when accessing a Sudoku board.
	/// </summary>
    [Serializable]
    public class BoardException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BoardException"/> class.
		/// </summary>
        public BoardException()
        {
        }

		/// <inheritdoc />
        public BoardException(string message)
            : base(message)
        {
        }

		/// <inheritdoc />
		public BoardException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

		/// <inheritdoc />
		protected BoardException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

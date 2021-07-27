using System.Runtime.Serialization;

namespace TicTacToe.Logic
{
    public class BoardContentException : Exception
    {
        public BoardContentException() { }

        public BoardContentException(string? message) : base(message) { }

        public BoardContentException(string? message, Exception? innerException) : base(message, innerException) { }

        protected BoardContentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class AlreadySetException : BoardContentException
    {
        public AlreadySetException() { }

        public AlreadySetException(string? message) : base(message) { }

        public AlreadySetException(string? message, Exception? innerException) : base(message, innerException) { }

        protected AlreadySetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class SetOrderException : BoardContentException
    {
        public SetOrderException() { }

        public SetOrderException(string? message) : base(message) { }

        public SetOrderException(string? message, Exception? innerException) : base(message, innerException) { }

        protected SetOrderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

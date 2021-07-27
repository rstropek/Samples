using System.Diagnostics;
using System.Numerics;

namespace TicTacToe.Logic
{
    public static class BoardContentFactory
    {
        public static IBoardContent Create() => new BoardContent(new byte[3 * 3]);
    }

    public interface IBoardContent
    {
        byte[] Content { get; }

        byte this[(int col, int row) ix] => Get(ix.col, ix.row);

        byte Get(int col, int row);

        IBoardContent Set(byte value, int col, int row);
    }

    public readonly struct BoardContent : IBoardContent, IEquatable<BoardContent>
    {
        internal readonly byte[] content;

        internal BoardContent(byte[]? other)
        {
            Debug.Assert((other?.Length ?? 3 * 3) == 3 * 3);
            content = new byte[3 * 3];
            if (other != null) other.CopyTo(content, 0);
        }

        public readonly byte[] Content => content.ToArray();

        public readonly Span<byte> GetRow(int i)
            => i switch
            {
                0 => content[..3],
                1 => content[3..^3],
                2 => content[^3..],
                _ => throw new ArgumentOutOfRangeException(nameof(i))
            };

        private static int CalculateIndexUnchecked(int col, int row) => row * 3 + col;
        internal static int CalculateIndex(int col, int row)
        {
            if (col is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(col));
            if (row is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(row));

            return CalculateIndexUnchecked(col, row);
        }

        internal static bool IsValidValue(byte value) => value is SquareContent.X or SquareContent.Y;

        public readonly byte Get(int col, int row) => content[CalculateIndex(col, row)];

        public readonly IBoardContent Set(byte value, int col, int row)
        {
            if (!IsValidValue(value)) throw new ArgumentOutOfRangeException(nameof(value));

            var arrIx = CalculateIndex(col, row);
            var newContent = new byte[3 * 3];
            content[..arrIx].CopyTo(newContent, 0);
            newContent[arrIx] = value;
            content[(arrIx + 1)..].CopyTo(newContent[(arrIx + 1)..].AsSpan());

            return new BoardContent(newContent);
        }

        #region Equatable-related members
        public readonly bool Equals(BoardContent other) => other.content.SequenceEqual(content);

        public readonly bool Equals(byte[] other) => other.SequenceEqual(content);

        public readonly override bool Equals(object? obj) => obj is BoardContent c && Equals(c);

        public readonly override int GetHashCode() => new BigInteger(content).GetHashCode();

        public static bool operator ==(BoardContent left, BoardContent right) => left.Equals(right);

        public static bool operator !=(BoardContent left, BoardContent right) => !(left == right);
        #endregion
    }
}

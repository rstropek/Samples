using System.Diagnostics;

namespace TicTacToe.Logic
{
    public record Game(string Player1, string Player2)
    {
        private IBoardContent content = BoardContentFactory.Create();
        private readonly object contentLockObject = new();
        private byte currentPlayer = 1;
        private readonly string[] players = new string[] { Player1, Player2 };

        internal Game(string Player1, string Player2, IBoardContent content) : this(Player1, Player2)
        {
            this.content = content;
        }

        internal static bool IsValid(IEnumerable<byte> content)
        {
            static int ToAggregatable(byte value)
                => value switch
                {
                    SquareContent.X => 1,
                    SquareContent.Y => -1,
                    SquareContent.EMPTY => 0,
                    _ => throw new InvalidOperationException("Invalid content, must NEVER happen"),
                };
            return content.Sum(ToAggregatable) is >= -1 and <= 1;
        }

        public void Set(int col, int row)
        {
            lock (contentLockObject)
            {
                if (content.Get(col, row) != SquareContent.EMPTY) throw new AlreadySetException();
                var newContent = content.Set(currentPlayer, col, row);
                if (!IsValid(newContent.Content)) throw new SetOrderException();
                content = newContent;
                currentPlayer = currentPlayer == 1 ? (byte)2 : (byte)1;
            }
        }

        public string WhoIsNext => players[currentPlayer - 1];

        internal string GetPlayerByContent(int col, int row) => players[content.Get(col, row) - 1];

        internal string? GetWinnerFromRows()
        {
            for (byte row = 0; row < 3; row++)
            {
                if (content.Get(0, row) != SquareContent.EMPTY && content.Get(0, row) == content.Get(1, row) 
                    && content.Get(0, row) == content.Get(2, row))
                {
                    return GetPlayerByContent(0, row);
                }
            }

            return null;
        }

        internal string? GetWinnerFromColumns()
        {
            for (byte col = 0; col < 3; col++)
            {
                if (content.Get(col, 0) != SquareContent.EMPTY && content.Get(col, 0) == content.Get(col, 1) 
                    && content.Get(col, 0) == content.Get( col, 2))
                {
                    return GetPlayerByContent(col, 0);
                }
            }

            return null;
        }

        internal string? GetWinnerFromDiagonals()
        {
            if (content.Get(1, 1) != SquareContent.EMPTY
                && (content.Get(1, 1) == content.Get(0, 0) && content.Get(1, 1) == content.Get(2, 2)
                    || content.Get(1, 1) == content.Get(0, 2) && content.Get(1, 1) == content.Get(2, 0)))
            {
                return GetPlayerByContent(1, 1);
            }

            return null;
        }

        public string? GetWinner() 
            => GetWinnerFromRows() ?? GetWinnerFromColumns() ?? GetWinnerFromDiagonals();
    }
}

namespace Ref;

// Implement a simple TTT board
// We do NOT store the board's data ourselves. Instead, we store a reference to the board's data
struct TicTacToeBoard(char[] board)
{
    // We support indexing the board using a tuple
    public readonly ref char this[(int row, int column) index] => ref board[index.row * 3 + index.column];

    // We also can do winner detection
    public readonly char GetWinner()
    {
        if (board[0] != ' ' && board[0] == board[1] && board[1] == board[2]) return board[0];
        if (board[3] != ' ' && board[3] == board[4] && board[4] == board[5]) return board[3];
        if (board[6] != ' ' && board[6] == board[7] && board[7] == board[8]) return board[6];
        if (board[0] != ' ' && board[0] == board[3] && board[3] == board[6]) return board[0];
        if (board[1] != ' ' && board[1] == board[4] && board[4] == board[7]) return board[1];
        if (board[2] != ' ' && board[2] == board[5] && board[5] == board[8]) return board[2];
        if (board[0] != ' ' && board[0] == board[4] && board[4] == board[8]) return board[0];
        if (board[2] != ' ' && board[2] == board[4] && board[4] == board[6]) return board[2];
        return ' ';
    }
}

// Alternative implementation based on Span<char>
// We need to make our struct ref to be able to store a Span<char> reference.
// It can be read-only because we do not modify the Span<char> reference after
// the struct has been created.
readonly ref struct TicTacToeBoardWithSpans(Span<char> board)
{
    private readonly Span<char> board = board;

    public readonly ref char this[(int row, int column) index] => ref board[index.row * 3 + index.column];

    public readonly char GetWinner()
    {
        if (board[0] != ' ' && board[0] == board[1] && board[1] == board[2]) return board[0];
        if (board[3] != ' ' && board[3] == board[4] && board[4] == board[5]) return board[3];
        if (board[6] != ' ' && board[6] == board[7] && board[7] == board[8]) return board[6];
        if (board[0] != ' ' && board[0] == board[3] && board[3] == board[6]) return board[0];
        if (board[1] != ' ' && board[1] == board[4] && board[4] == board[7]) return board[1];
        if (board[2] != ' ' && board[2] == board[5] && board[5] == board[8]) return board[2];
        if (board[0] != ' ' && board[0] == board[4] && board[4] == board[8]) return board[0];
        if (board[2] != ' ' && board[2] == board[4] && board[4] == board[6]) return board[2];
        return ' ';
    }
}

public static class RefIterators
{
    static IEnumerable<bool> ApplyTicTacToeMoves(char[] board, (int row, int column, char player)[] moves)
    {
        var ttt = new TicTacToeBoard(board);
        foreach (var (row, column, player) in moves)
        {
            // The following line does NOT work prior to C# 13 (try it at https://dotnetfiddle.net/ZU9df0).
            // New feature: ref and unsafe (not demoed here) in iterators
            ref var field = ref ttt[(row, column)];

            // You think, you would not need the ref here? Well, try replacing 
            // the line above with the following line and see what happens. Why
            // doesn't it work?
            //var field = ttt[(row, column)];

            field = player;
            yield return ttt.GetWinner() != ' ';
        }
    }

    static IEnumerable<bool> ApplyTicTacToeMovesWithSpans(/*Span<char>*/ Memory<char> board, (int row, int column, char player)[] moves)
    {
        // Creating a new TicTacToeBoardWithSpans here does NOT work because it would be
        // preserved across yield boundaries. This is not allowed for ref structs and/or ref variables.
        //var ttt = new TicTacToeBoardWithSpans(board.Span);
        foreach (var (row, column, player) in moves)
        {
            // We can create the TicTacToeBoardWithSpans instance here.
            //
            // However, board cannot be a Span<char> because it would be preserved across yield boundaries.
            // So we use Memory<char> instead. However, that means that we cannot put the
            // board content on the stack.
            var ttt = new TicTacToeBoardWithSpans(board.Span);
            ref var field = ref ttt[(row, column)];
            field = player;
            yield return ttt.GetWinner() != ' ';
        }
    }

    public static void Iterators()
    {
        char[] board = [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '];
        var moves = new[] { (0, 0, 'X'), (1, 1, 'O'), (0, 1, 'X'), (1, 0, 'O'), (0, 2, 'X') };
        foreach (var hasWinner in ApplyTicTacToeMoves(board, moves))
        {
            Console.WriteLine(hasWinner);
        }

        Memory<char> boardSpan = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        foreach (var hasWinner in ApplyTicTacToeMovesWithSpans(boardSpan, moves))
        {
            Console.WriteLine(hasWinner);
        }
    }
}

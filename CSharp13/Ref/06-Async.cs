namespace Ref;

public static class RefAsync
{
    public static async Task<char[]> GetTTTBoardAsync()
    {
        await Task.Delay(100);
        return [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '];
    }

    public static async Task UseTTTBoardWithAsync()
    {
        var board = await GetTTTBoardAsync();
        var ttt = new TicTacToeBoard(board);
        var moves = new[] { (0, 0, 'X'), (1, 1, 'O'), (0, 1, 'X'), (1, 0, 'O'), (0, 2, 'X') };
        foreach (var (row, column, player) in moves)
        {
            // This is allowed starting in C# 13. Previoulsy, by-ref locals variables
            // were not allowed in async methods (try it at https://dotnetfiddle.net/zBBB1K).
            ref var field = ref ttt[(row, column)];
            field = player;
            await Task.Delay(100);
        }

        Console.WriteLine(ttt.GetWinner());
    }
}
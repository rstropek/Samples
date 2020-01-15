namespace TicTacToe.Logic
{
    public interface IBoardSerializer
    {
        string Serialize(Board board);
        Board Deserialize(string serializedBoard);
    }
}
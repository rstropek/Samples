namespace Samples.Sudoku
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;

	public interface IBoardRepository
	{
		Task Save(string boardName, Board board);

		Task<Board> Load(string boardName);
	}
}

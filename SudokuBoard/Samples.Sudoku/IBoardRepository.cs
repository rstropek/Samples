namespace Samples.Sudoku
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;

	public interface IBoardRepository
	{
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Runtime too long for prop")]
		Task<IEnumerable<string>> GetBoardNamesAsync();

		Task Save(string boardName, Board board);

		Task<Board> Load(string boardName);
	}
}

namespace Samples.Sudoku
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Threading.Tasks;
	
	public interface IBoardReaderWriter
	{
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Runtime too long for prop")]
		
		Task<IEnumerable<string>> GetBoardNamesAsync();

		Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode);
	}
}

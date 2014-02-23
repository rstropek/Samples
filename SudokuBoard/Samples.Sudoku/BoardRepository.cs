namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	public class BoardRepository
	{
		private IBoardReaderWriter readerWriter;

		public BoardRepository(IBoardReaderWriter readerWriter)
		{
			ContractExtensions.IsNotNull(readerWriter, "readerWriter");
			Contract.EndContractBlock();

			this.readerWriter = readerWriter;
		}

		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Runtime too long for prop")]
		public Task<IEnumerable<string>> GetBoardNamesAsync()
		{
			Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

			return this.readerWriter.GetBoardNamesAsync();
		}

		public async Task SaveAsync(string boardName, Board board)
		{
			ContractExtensions.IsNotNull(boardName, "boardName");
			ContractExtensions.IsNotNull(board, "board");
			Contract.EndContractBlock();

			using (var stream = await this.readerWriter.OpenStreamAsync(boardName, AccessMode.Write))
			{
				var boardData = (byte[])board;
				await stream.WriteAsync(boardData, 0, boardData.Length).ConfigureAwait(false);
			}
		}

		public async Task<Board> LoadAsync(string boardName)
		{
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.EndContractBlock();

			using (var stream = await this.readerWriter.OpenStreamAsync(boardName, AccessMode.Read))
			{
				var boardData = new byte[9 * 9];
				var resultLength = await stream.ReadAsync(boardData, 0, boardData.Length).ConfigureAwait(false);
				if (resultLength != 9 * 9)
				{
					throw new BoardException("Incorrect file format. Board file too small.");
				}

				var dummyBuffer = new byte[1];
				if (await stream.ReadAsync(dummyBuffer, 0, 1) > 0)
				{
					throw new BoardException("Incorrect file format. Board file too long.");
				}

				return (Board)boardData;
			}
		}
	}
}

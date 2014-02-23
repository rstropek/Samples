namespace Samples.Sudoku
{
	using System.Diagnostics.Contracts;
	using System.Threading.Tasks;

	/// <summary>
	/// Contains methods to read/write <see cref="Board"/> instances.
	/// </summary>
	public class BoardStreamRepository
	{
		private IStreamInitializer readerWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoardStreamRepository"/> class.
		/// </summary>
		/// <param name="readerWriter">The underlying reader/writer.</param>
		public BoardStreamRepository(IStreamInitializer readerWriter)
		{
			ContractExtensions.IsNotNull(readerWriter, "readerWriter");
			Contract.Ensures(this.readerWriter != null);
			Contract.EndContractBlock();

			this.readerWriter = readerWriter;
		}

		/// <summary>
		/// Saves the board using the specified name
		/// </summary>
		/// <param name="boardName">Name of the board.</param>
		/// <param name="board">The board to save.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public async Task SaveAsync(string boardName, Board board)
		{
			ContractExtensions.IsNotNull(boardName, "boardName");
			ContractExtensions.IsNotNull(board, "board");
			Contract.Ensures(Contract.Result<Task>() != null);
			Contract.EndContractBlock();

			// Open underlying stream for writing
			using (var stream = await this.readerWriter.OpenStreamAsync(boardName, AccessMode.Write))
			{
				// Get byte array from board and save it.
				var boardData = (byte[])board;

				// Note the use of ConfigureAwait(false) here.
				await stream.WriteAsync(boardData, 0, boardData.Length)
					.ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Loads the board using the specified name
		/// </summary>
		/// <param name="boardName">Name of the board to load.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The value of the 
		/// TResult parameter contains the loaded board.
		/// </returns>
		public async Task<Board> LoadAsync(string boardName)
		{
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.Ensures(Contract.Result<Task<Board>>() != null);
			Contract.Ensures(Contract.Result<Task<Board>>().Result != null);
			Contract.EndContractBlock();

			// Open underlying stream for reading
			using (var stream = await this.readerWriter.OpenStreamAsync(boardName, AccessMode.Read))
			{
				// Load board content
				var boardData = new byte[9 * 9];
				var resultLength = await stream.ReadAsync(boardData, 0, boardData.Length).ConfigureAwait(false);
				if (resultLength != 9 * 9)
				{
					// Board stream has to be at least 9 * 9 bytes long.
					throw new BoardException("Incorrect file format. Board file too small.");
				}

				// Convert board data. This will throw an exception if the board data is invalid.
				return (Board)boardData;
			}
		}
	}
}

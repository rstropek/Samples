namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Implements a reader/writer for board data which stores boards in files.
	/// </summary>
	public class FileReaderWriter : IStreamInitializer
	{
		// Note the use of Lazy<T> here.
		private Lazy<string> boardsDirectory = null;

		public FileReaderWriter()
		{
			this.boardsDirectory = new Lazy<string>(FileReaderWriter.GetBoardsDirectory);
		}

		/// <inheritdoc />
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller responsible for disposing")]
		public Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode)
		{
			Contract.Ensures(Contract.Result<Task<Stream>>().Result != null);
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.EndContractBlock();

			return Task.FromResult(new FileStream(
				Path.Combine(this.boardsDirectory.Value, boardName),
				accessMode == AccessMode.Read ? FileMode.Open : FileMode.OpenOrCreate) as Stream);
		}

		private static string GetBoardsDirectory()
		{
			Contract.Ensures(Contract.Result<string>() != null);
			Contract.Ensures(Contract.Result<string>().Length > 0);

			var appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
			return Path.Combine(appDataPath, "Boards");
		}
	}
}

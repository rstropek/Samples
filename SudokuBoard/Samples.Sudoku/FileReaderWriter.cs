namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	
	public class FileReaderWriter : IBoardReaderWriter
	{
		private Lazy<string> boardsDirectory = null;

		public FileReaderWriter()
		{
			this.boardsDirectory = new Lazy<string>(FileReaderWriter.GetBoardsDirectory);
		}

		public Task<IEnumerable<string>> GetBoardNamesAsync()
		{
			Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>() != null);
			Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>().Result != null);
			Contract.Ensures(Contract.ForAll(Contract.Result<Task<IEnumerable<string>>>().Result, boardName => boardName != null));
			Contract.Ensures(Contract.ForAll(Contract.Result<Task<IEnumerable<string>>>().Result, boardName => boardName.Length > 0));

			return Task.Run(() => Directory.GetFiles(this.boardsDirectory.Value, "*.board").AsEnumerable());
		}

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

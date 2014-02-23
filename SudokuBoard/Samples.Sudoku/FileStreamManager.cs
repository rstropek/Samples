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
	/// Stream manager implementation for local files.
	/// </summary>
	public class FileStreamManager : IStreamManager
	{
		// Note the use of Lazy<T> here.
		private Lazy<string> boardsDirectory = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileStreamManager"/> class.
		/// </summary>
		/// <remarks>
		/// Boards will be stored in the subdirectory "Boards" in the roaming application data
		/// directory.
		/// </remarks>
		public FileStreamManager()
		{
			Contract.Ensures(this.boardsDirectory != null);

			this.boardsDirectory = new Lazy<string>(FileStreamManager.GetBoardsDirectory);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileStreamManager"/> class.
		/// </summary>
		/// <param name="boardsDirectory">Directory where the boards should be stored.</param>
		public FileStreamManager(string boardsDirectory)
		{
			Contract.Ensures(this.boardsDirectory != null);

			this.boardsDirectory = new Lazy<string>(() => boardsDirectory);
		}

		// Note the use of inheritdoc here. Documentation is inherited from interface.
		/// <inheritdoc />
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller responsible for disposing")]
		public Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode)
		{
			// Note that we do not need any preconditions here. They are inherited from
			// the base interface.

			return Task.FromResult(new FileStream(
				Path.Combine(this.boardsDirectory.Value, boardName),
				accessMode == AccessMode.Read ? FileMode.Open : FileMode.OpenOrCreate) as Stream);
		}

		private static string GetBoardsDirectory()
		{
			Contract.Ensures(Contract.Result<string>() != null);
			Contract.Ensures(Contract.Result<string>().Length > 0);

			var appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

			// Note: Try this method in the debugger and check out the new features of the
			// Autos window.
			return Path.Combine(appDataPath, "Boards");
		}
	}
}

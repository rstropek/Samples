namespace Samples.Sudoku
{
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Auth;
	using System.Diagnostics.Contracts;
	using System.Threading.Tasks;

	/// <summary>
	/// Implements helper methods to load board data.
	/// </summary>
	public static class BoardReader
	{
		/// <summary>
		/// Loads board from file asynchronously.
		/// </summary>
		/// <param name="boardName">Name of the board.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The value of the 
		/// TResult parameter contains the loaded board.
		/// </returns>
		public static Task<Board> LoadFromFileAsync(string boardName, string boardsDirectory = null)
		{
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.EndContractBlock();

			var streamManager = boardsDirectory == null ? new FileStreamManager() : new FileStreamManager(boardsDirectory);
			var repository = new BoardStreamRepository(streamManager);
			return repository.LoadAsync(boardName);
		}

		/// <summary>
		/// Loads board from Azure Blob Storage asynchronously.
		/// </summary>
		/// <param name="boardName">Name of the board.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The value of the 
		/// TResult parameter contains the loaded board.
		/// </returns>
		public static Task<Board> LoadFromBlobStorageAsync(string accountName, string accountKey, string containerName, string boardName)
		{
			ContractExtensions.IsNotNull(accountName, "accountName");
			ContractExtensions.IsNotNull(accountKey, "accountKey");
			ContractExtensions.IsNotNull(containerName, "containerName");
			Contract.EndContractBlock();

			var credentials = new StorageCredentials(accountName, accountKey);
			var account = new CloudStorageAccount(credentials, true);
			var client = account.CreateCloudBlobClient();
			var container = client.GetContainerReference(containerName);
			var repository = new BoardStreamRepository(new CloudBlobStreamManager(container));
			return repository.LoadAsync(boardName);
		}
	}
}

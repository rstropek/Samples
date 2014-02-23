namespace Samples.Sudoku
{
	using Microsoft.WindowsAzure.Storage.Blob;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading.Tasks;

	/// <summary>
	/// Implements a reader/writer for board data which stores boards in Azure Blob Storage.
	/// </summary>
	public class CloudBlobReaderWriter : IStreamInitializer
	{
		private CloudBlobContainer container;

		/// <summary>
		/// Initializes a new instance of the <see cref="CloudBlobReaderWriter"/> class.
		/// </summary>
		/// <param name="container">The container in which the boards should be stored.</param>
		public CloudBlobReaderWriter(CloudBlobContainer container)
		{
			ContractExtensions.IsNotNull(container, "container");
			Contract.Ensures(this.container != null);
			Contract.EndContractBlock();

			this.container = container;
		}

		/// <inheritdoc />
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller responsible for disposing")]
		public async Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode)
		{
			Contract.Ensures(Contract.Result<Task<Stream>>() != null);
			Contract.Ensures(Contract.Result<Task<Stream>>().Result != null);
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.EndContractBlock();

			// Get reference to requested blob and open stream
			var blob = this.container.GetBlockBlobReference(boardName);
			return accessMode == AccessMode.Read
				? await blob.OpenReadAsync().ConfigureAwait(false)
				: await blob.OpenWriteAsync().ConfigureAwait(false);
		}
	}
}

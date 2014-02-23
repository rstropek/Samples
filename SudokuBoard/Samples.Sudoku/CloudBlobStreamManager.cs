namespace Samples.Sudoku
{
	using Microsoft.WindowsAzure.Storage.Blob;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading.Tasks;

	/// <summary>
	/// Stream manager implementation for Windows Azure Blob Storage.
	/// </summary>
	public class CloudBlobStreamManager : IStreamManager
	{
		private CloudBlobContainer container;

		/// <summary>
		/// Initializes a new instance of the <see cref="CloudBlobStreamManager"/> class.
		/// </summary>
		/// <param name="container">The container in which the boards should be stored.</param>
		public CloudBlobStreamManager(CloudBlobContainer container)
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
			// Note that we do not need any preconditions here. They are inherited from
			// the base interface.

			// Get reference to requested blob and open stream
			var blob = this.container.GetBlockBlobReference(boardName);
			return accessMode == AccessMode.Read
				? await blob.OpenReadAsync().ConfigureAwait(false)
				: await blob.OpenWriteAsync().ConfigureAwait(false);
		}
	}
}

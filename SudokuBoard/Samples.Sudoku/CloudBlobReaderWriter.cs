namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Blob;
	
	public class CloudBlobReaderWriter : IBoardReaderWriter
	{
		private CloudBlobContainer container;

		public CloudBlobReaderWriter(CloudBlobContainer container)
		{
			ContractExtensions.IsNotNull(container, "container");
			Contract.EndContractBlock();

			this.container = container;
		}

		public async Task<IEnumerable<string>> GetBoardNamesAsync()
		{
			Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>() != null);
			Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>().Result != null);
			Contract.Ensures(Contract.ForAll(Contract.Result<Task<IEnumerable<string>>>().Result, boardName => boardName != null));
			Contract.Ensures(Contract.ForAll(Contract.Result<Task<IEnumerable<string>>>().Result, boardName => boardName.Length > 0));

			var result = new List<string>();
			BlobContinuationToken token = null;
			BlobResultSegment resultSegment;
			do
			{
				resultSegment = await this.container.ListBlobsSegmentedAsync(token);
				result.AddRange(resultSegment.Results.Select(blob => blob.Uri.ToString()));
			}
			while (resultSegment.ContinuationToken != null);
			return result;
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller responsible for disposing")]
		public async Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode)
		{
			Contract.Ensures(Contract.Result<Task<Stream>>().Result != null);
			ContractExtensions.IsNotNull(boardName, "boardName");
			Contract.EndContractBlock();

			var blob = this.container.GetBlockBlobReference(boardName);
			if (accessMode == AccessMode.Read)
			{
				return await blob.OpenReadAsync();
			}
			else
			{
				return await blob.OpenWriteAsync();
			}
		}
	}
}

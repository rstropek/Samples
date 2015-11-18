namespace Samples.Sudoku.Test
{
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Blob.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Fakes;
    using System.Globalization;
    using System.IO;
    using System.IO.Fakes;
    using System.Linq;
    using System.Net;
    using System.Net.Fakes;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Tests for stream manager classes
    /// </summary>
    [TestClass]
	public class StreamManagerTest
	{
		private const string dummyContainerName = "dummycontainer";
        private static readonly string dummyContainerUri = $"http://dummystorage.blob.core.windows.net/{dummyContainerName}";
		private const string dummyBoardName = "Dummyboard";
		private const string dummyStorageName = "dummystorage";

		[TestMethod]
		[TestCategory("Integration")]
		public async Task AzureStorageIntegrationTest()
		{
			const string sampleBlobName = "Testblob";

			// Read configuration settings
			var storageName = ConfigurationManager.AppSettings["StorageName"];
			var storageKey = ConfigurationManager.AppSettings["StorageKey"];
			var containerName = ConfigurationManager.AppSettings["ContainerName"];

			// Create normed sample board in blob storage
			var container = GetContainerReference(storageName, storageKey, containerName);
			await container.CreateIfNotExistsAsync();
			var blob = container.GetBlockBlobReference(sampleBlobName);
			if (!await blob.ExistsAsync())
			{
				await blob.UploadFromByteArrayAsync(BoardSampleData.sampleBoard, 0, BoardSampleData.sampleBoard.Length);
			}

			// Execute test
			await AzureStorageIntegrationTestInternal(storageName, storageKey, containerName, sampleBlobName);
		}

		private CloudBlobContainer GetContainerReference(string storageName, string storageKey, string containerName)
		{
			var credentials = new StorageCredentials(storageName, storageKey);
			var account = new CloudStorageAccount(credentials, true);
			var blobClient = account.CreateCloudBlobClient();
			return blobClient.GetContainerReference(containerName);
		}

		private async Task AzureStorageIntegrationTestInternal(string storageName, string storageKey, string containerName, string boardName)
		{
			var repository = new BoardStreamRepository(
				new CloudBlobStreamManager(GetContainerReference(storageName, storageKey, containerName)));
			var board = await repository.LoadAsync(boardName);

			CollectionAssert.AreEqual(BoardSampleData.sampleBoard, (byte[])board);
		}

		[TestMethod]
		[TestCategory("With fakes")]
		public async Task CloudBlobStreamManagerShimmedLoadTest()
		{
			// Note that we use shims from Microsoft Fakes to simulate Windows Azure Blob Storage.
			using (ShimsContext.Create())
			{
				// Setup shims
				ShimCloudBlobContainer.AllInstances.GetBlockBlobReferenceString = (container, blobName) =>
					{
						Assert.AreEqual(dummyContainerUri, container.Uri.AbsoluteUri);
						Assert.AreEqual(dummyBoardName, blobName);
						return new CloudBlockBlob(new Uri(dummyContainerUri));
					};
				ShimCloudBlob.AllInstances.OpenReadAsync = (blob) =>
					Task.FromResult(new MemoryStream(BoardSampleData.sampleBoard) as Stream);

				// Execute
				var repository = new BoardStreamRepository(new CloudBlobStreamManager(
					new CloudBlobContainer(new Uri(dummyContainerUri))));
				var result = await repository.LoadAsync(dummyBoardName);

				// Check result
				CollectionAssert.AreEqual(BoardSampleData.sampleBoard, (byte[])result);
			}
		}

		[TestMethod]
		[TestCategory("With fakes")]
		public async Task FileStreamManagerShimmedLoadTest()
		{
			using (ShimsContext.Create())
			{
				// Note how we use a shimmed constructor here.
				ShimFileStream.ConstructorStringFileMode = (@this, fileName, __) =>
					{
						Assert.IsTrue(fileName.EndsWith("\\AppData\\Roaming\\Boards\\" + dummyBoardName));
						new ShimFileStream(@this)
							{
								ReadAsyncByteArrayInt32Int32CancellationToken = (buffer, ___, ____, _____) =>
								{
									BoardSampleData.sampleBoard.CopyTo(buffer, 0);
									return Task.FromResult(BoardSampleData.sampleBoard.Length);
								}
							};
					};

				var repository = new BoardStreamRepository(new FileStreamManager());
				var result = await repository.LoadAsync(dummyBoardName);

				CollectionAssert.AreEqual(BoardSampleData.sampleBoard, (byte[])result);
			}
		}

		[TestMethod]
		[TestCategory("With fakes")]
		public async Task CloudBlobShimmedWebRequestTest()
		{
			// Setup blob to simulate
			var simulatedBlobs = new Dictionary<string, byte[]>();
			simulatedBlobs.Add($"/{dummyContainerName}/{dummyBoardName}", BoardSampleData.sampleBoard);

			await this.ExecuteWithShimmedWebRequestForBlockBlobsAsync(simulatedBlobs, async () =>
				{
                    var storageKey = "asdf"; // ConfigurationManager.AppSettings["StorageKey"];

					// Execute existing unit test, but this time with shims instead of real web requests
					await this.AzureStorageIntegrationTestInternal(dummyStorageName, storageKey, dummyContainerName, dummyBoardName);
				});
		}

		private async Task ExecuteWithShimmedWebRequestForBlockBlobsAsync(Dictionary<string, byte[]> simulatedBlobs, Func<Task> body)
		{
			// Note how we create the shims context here
			using (ShimsContext.Create())
			{
				// Azure storage uses IAsyncResult-pattern in the background. Therefore we have to create shims
				// for Begin/EndGetResponse. BTW - you can check the code of Azure Storage Library at
				// https://github.com/WindowsAzure/azure-storage-net
				ShimHttpWebRequest.AllInstances.BeginGetResponseAsyncCallbackObject = (@this, callback, state) =>
				{
					// Check if the request matches on of the blobs that we should simulate
					byte[] requestedBlob;
					if (!simulatedBlobs.TryGetValue(@this.RequestUri.AbsolutePath, out requestedBlob))
					{
						Assert.Fail("Unexpected request for {0}", @this.RequestUri.AbsoluteUri);
					}

					// Setup IAsyncResult; note how we use a stub for that.
					var result = new StubIAsyncResult()
					{
						// Azure Storage Library relies on a wait handle. We give one back that is immediately set.
						AsyncWaitHandleGet = () => new ManualResetEvent(true),

						// We pass on the state 
						AsyncStateGet = () => state
					};

					// We immediately call the callback as we do not have to wait for a real web request to finish
					callback(result);
					return result;
				};

				ShimHttpWebRequest.AllInstances.EndGetResponseIAsyncResult = (@this, __) =>
				{
					// Check if the request matches on of the blobs that we should simulate
					byte[] requestedBlob;
					if (!simulatedBlobs.TryGetValue(@this.RequestUri.AbsolutePath, out requestedBlob))
					{
						Assert.Fail("Unexpected request for {0}", @this.RequestUri.AbsoluteUri);
					}

					// Setup response headers. Read Azure Storage HTTP docs for details
					// (see http://msdn.microsoft.com/en-us/library/windowsazure/dd179440.aspx)
					var headers = new WebHeaderCollection();
					headers.Add("Accept-Ranges", "bytes");
					headers.Add("ETag", "0xFFFFFFFFFFFFFFF");
					headers.Add("x-ms-request-id", Guid.NewGuid().ToString());
					headers.Add("x-ms-version", "2013-08-15");
					headers.Add("x-ms-lease-status", "unlocked");
					headers.Add("x-ms-lease-state", "available");
					headers.Add("x-ms-blob-type", "BlockBlob");
					headers.Add("Date", DateTime.Now.ToString("R", CultureInfo.InvariantCulture));

					// Calculate MD5 hash for our blob and add it to the response headers
					var md5Check = MD5.Create();
					md5Check.TransformBlock(requestedBlob, 0, requestedBlob.Length, null, 0);
					md5Check.TransformFinalBlock(new byte[0], 0, 0);
					var hashBytes = md5Check.Hash;
					var hashVal = Convert.ToBase64String(hashBytes);
					headers.Add("Content-MD5", hashVal);

					// As the headers are complete, we can now build the shimmed web response
					return new ShimHttpWebResponse()
					{
						GetResponseStream = () =>
						{
							// Simulate downloaded bytes
							return new MemoryStream(requestedBlob);
						},

						// Status code depends on x-ms-range request header
						// (see Azure Storage HTTP docs for details)
						StatusCodeGet = () => string.IsNullOrEmpty(@this.Headers["x-ms-range"]) ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
						HeadersGet = () => headers,
						ContentLengthGet = () => requestedBlob.Length,
						ContentTypeGet = () => "application/octet-stream",
						LastModifiedGet = () => new DateTime(2014, 1, 1)
					};
				};

				await body();
			}
		}
	}
}

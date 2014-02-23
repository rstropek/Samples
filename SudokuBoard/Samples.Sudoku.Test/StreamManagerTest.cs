namespace Samples.Sudoku.Test
{
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Auth;
	using Microsoft.WindowsAzure.Storage.Blob;
	using Microsoft.WindowsAzure.Storage.Blob.Fakes;
	using Samples.Sudoku.Fakes;
	using System;
	using System.Configuration;
	using System.IO;
	using System.IO.Fakes;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Tests for stream manager classes
	/// </summary>
	[TestClass]
	public class StreamManagerTest
	{
		private const string dummyContainerUri = "http://dummystorage.blob.core.windows.net/dummycontainer";
		private const string dummyBoardName = "Dummy";

		[TestMethod]
		public async Task CloudBlobStreamManagerShimmedLoadTest()
		{
			// Note that we use shims from Microsoft Fakes to simulate Windows Azure Blob Storage.
			using(ShimsContext.Create())
			{
				// Setup shims
				ShimCloudBlobContainer.AllInstances.GetBlockBlobReferenceString = (container, blobName) =>
					{
						Assert.AreEqual(dummyContainerUri, container.Uri.AbsoluteUri);
						Assert.AreEqual(dummyBoardName, blobName);
						return new CloudBlockBlob(new Uri(dummyContainerUri));
					};
				ShimCloudBlockBlob.AllInstances.OpenReadAsync = (blob) =>
					Task.FromResult(new MemoryStream(BoardSampleData.sampleBoard) as Stream);

				// Execute
				var repository = new BoardStreamRepository(new CloudBlobStreamManager(
					new CloudBlobContainer(new Uri(dummyContainerUri))));
				var result = await repository.LoadAsync(dummyBoardName);

				// Check result
				Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])result));
			}
		}

		[TestMethod]
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

				Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])result));
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task AzureStorageIntegrationTest()
		{
			var storageName = ConfigurationManager.AppSettings["StorageName"];
			var storageKey = ConfigurationManager.AppSettings["StorageKey"];
			var containerName = ConfigurationManager.AppSettings["ContainerName"];

			var credentials = new StorageCredentials(storageName, storageKey);
			var account = new CloudStorageAccount(credentials, true);
			var blobClient = account.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference(containerName);

			var repository = new BoardStreamRepository(new CloudBlobStreamManager(container));
			await repository.SaveAsync("TestBoard", (Board)BoardSampleData.sampleBoard);

			var board = await repository.LoadAsync("TestBoard");
			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])board));
		}
	}
}

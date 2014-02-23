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
using System.Linq;
using System.Threading.Tasks;

	[TestClass]
	public class BoardRepositoryTest
	{
		[TestMethod]
		public async Task TestGetBoardNames()
		{
			var stub = new StubIBoardReaderWriter();
			stub.GetBoardNamesAsync = () => Task.FromResult(new[] { "Board1", "Board2" }.AsEnumerable());

			var repository = new BoardRepository(stub);
			var boardNames = (await repository.GetBoardNamesAsync()).ToArray();
			Assert.AreEqual(2, boardNames.Length);
			Assert.AreEqual("Board1", boardNames[0]);
			Assert.AreEqual("Board2", boardNames[1]);

			stub.GetBoardNamesAsync = () => Task.FromResult(new string[] { }.AsEnumerable());
			boardNames = (await repository.GetBoardNamesAsync()).ToArray();
			Assert.AreEqual(0, boardNames.Length);
		}

		[TestMethod]
		public async Task TestLoadBoard()
		{
			var stub = new StubIBoardReaderWriter();
			stub.OpenStreamAsyncStringAccessMode = (_, __) =>
				Task.FromResult(new MemoryStream(BoardSampleData.sampleBoard) as Stream);

			var repository = new BoardRepository(stub);
			var board = await repository.LoadAsync("DummyBoardName");
			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])board));

			stub.OpenStreamAsyncStringAccessMode = (_, __) =>
				Task.FromResult(new MemoryStream(new byte[] { 1, 2 }) as Stream);
			await AssertExtensions.ThrowsExceptionAsync<Exception>(async () => await repository.LoadAsync("DummyBoardName"));

			stub.OpenStreamAsyncStringAccessMode = (_, __) =>
				Task.FromResult(new MemoryStream(Enumerable.Range(0, 9 * 9 + 1).Cast<byte>().ToArray()) as Stream);
			await AssertExtensions.ThrowsExceptionAsync<Exception>(async () => await repository.LoadAsync("DummyBoardName"));
		}

		[TestMethod]
		public async Task TestSaveBoard()
		{
			var buffer = new byte[9 * 9];
			var stub = new StubIBoardReaderWriter();
			stub.OpenStreamAsyncStringAccessMode = (_, __) =>
				Task.FromResult(new MemoryStream(buffer) as Stream);

			var repository = new BoardRepository(stub);
			await repository.SaveAsync("DummyBoardName", (Board)BoardSampleData.sampleBoard);
			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual(buffer));
		}

		[TestMethod]
		public async Task CloudBlobWriterOpenStreamTest()
		{
			using(ShimsContext.Create())
			{
				CloudBlockBlob dummyBlob = null;
				ShimCloudBlobContainer.AllInstances.GetBlockBlobReferenceString = (_, __) =>
					dummyBlob = new CloudBlockBlob(new Uri("http://dummystorage.blob.core.windows.net/dummycontainer"));

				ShimCloudBlockBlob.AllInstances.OpenReadAsync = (blob) =>
					{
						if (blob == dummyBlob)
						{
							return Task.FromResult(new MemoryStream(BoardSampleData.sampleBoard) as Stream);
						}
						else
						{
							Assert.Fail("Unknown blob");
							return null;
						}
					};

				var repository = new BoardRepository(new CloudBlobReaderWriter(
					new CloudBlobContainer(new Uri("http://dummy.blob.com"))));

				Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual(
					(byte[])(await repository.LoadAsync("Dummy"))));
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

			var repository = new BoardRepository(new CloudBlobReaderWriter(container));
			await repository.SaveAsync("TestBoard", (Board)BoardSampleData.sampleBoard);

			var board = await repository.LoadAsync("TestBoard");
			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])board));
		}
	}
}

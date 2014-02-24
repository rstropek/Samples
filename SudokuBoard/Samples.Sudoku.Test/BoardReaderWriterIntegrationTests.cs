namespace Samples.Sudoku.Test
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	/// <summary>
	/// Integration tests for reading/writing board data.
	/// </summary>
	[TestClass]
	public class BoardReaderWriterIntegrationTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task TestLoadBoardFromFile()
		{
			const string sampleBoardName = "SampleBoard";

			var directory = Path.Combine(this.TestContext.TestDir, "Boards");
			await Task.Run(() => Directory.CreateDirectory(directory));
			using (var stream = new FileStream(Path.Combine(directory, sampleBoardName), FileMode.CreateNew))
			{
				await stream.WriteAsync(BoardSampleData.sampleBoard, 0, BoardSampleData.sampleBoard.Length);
			}

			var board = await BoardReader.LoadFromFileAsync(sampleBoardName, directory);
			Assert.IsTrue(((byte[])board).SequenceEqual(BoardSampleData.sampleBoard));
		}
	}
}

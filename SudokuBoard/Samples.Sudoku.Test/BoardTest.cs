using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Samples.Sudoku.Test
{
    [TestClass]
    public class BoardTest
    {
		/// <summary>
		/// Tests type cast operators for <see cref="Samples.Sudoku.Board"/> class.
		/// </summary>
        [TestMethod]
        public void TestBoardTypeCasts()
		{
			#region Example for type cast operators
			byte[] boardData = BoardSampleData.sampleBoard;

			// Cast bytes to Board instance
			using (Board board = (Board)boardData)
			{
				// Cast board instance back to bytes
				byte[] resultingBoardData = (byte[])board;

				// Content of boardData is equal to content of resultingBoardData
				Assert.IsTrue(resultingBoardData.SequenceEqual(boardData));
			}
			#endregion

			// Test if type converter throw exception in case of invalid board data.
			var invalidBoard = new byte[9 * 9];
			Array.Copy(BoardSampleData.sampleBoard, invalidBoard, 9 * 9);
			invalidBoard[9 * 9 - 1] = 1;
			AssertExtensions.ThrowsException<BoardException>(() => { var board = (Board)invalidBoard; });
		}

		/// <summary>
		/// Tests methods used to validate board values.
		/// </summary>
		/// <returns>Task representing the async test.</returns>
		[TestMethod]
		public async Task TestIsValuePossibleAsync()
		{
			// Note that we do not use an instance of the Board class for testing.
			// The validation functions are pure functions and can be tested individually
			// without the rest of the infrastructure of the Board class.

			// Note the use of PrivateType to access private members during unit testing.
			var pt = new PrivateType(typeof(Board));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardSampleData.sampleBoard, 8, (byte)9));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardSampleData.sampleBoard, 8, (byte)9));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 0, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 0, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 3, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 3, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 8, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardSampleData.sampleBoard, 8, 8, (byte)9));

			// Note the use of the await keyword here. This will result in an async unit test.
			// Remember to change the return value of the test method to Task. If you forget to do so,
			// your unit test will fail.
			Assert.IsTrue(await (Task<bool>)pt.InvokeStatic("IsValuePossibleAsync", BoardSampleData.sampleBoard, 0, 0, (byte)1));
			Assert.IsFalse(await (Task<bool>)pt.InvokeStatic("IsValuePossibleAsync", BoardSampleData.sampleBoard, 0, 0, (byte)2));
		}

		/// <summary>
		/// Tests copying a row of the board.
		/// </summary>
		[TestMethod]
		public void TestCopyOfRow()
		{
			using (var board = (Board)BoardSampleData.sampleBoard)
			{
				// Test copying first row
				var row = (byte[])board[0];
				Assert.IsTrue(row.SequenceEqual(BoardSampleData.sampleBoard.Take(9)));

				// Test copying a row in the middle
				row = (byte[])board[1];
				Assert.IsTrue(row.SequenceEqual(BoardSampleData.sampleBoard.Skip(9).Take(9)));

				// Test copying the last row
				row = (byte[])board[8];
				Assert.IsTrue(row.SequenceEqual(BoardSampleData.sampleBoard.Skip(8 * 9).Take(9)));

				// Test if argument is validated
				AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => { var r = (byte[])board[9]; });
			}
		}

		/// <summary>
		/// Tests setting of cell values.
		/// </summary>
        [TestMethod]
        public void TestSetCell()
        {
            using (var board = new Board())
            {
				// Test TrySetCell
				for (var i = 0; i < BoardSampleData.sampleBoard.Length; i++)
                {
					if (BoardSampleData.sampleBoard[i] != 0)
                    {
						Assert.IsTrue(board.TrySetCell(i / 9, i % 9, BoardSampleData.sampleBoard[i]));
                    }
                }

				Assert.IsTrue(((byte[])board).SequenceEqual(BoardSampleData.sampleBoard));

				// Test parameter validation
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(9, 0, 1));
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(0, 9, 1));
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(0, 0, 10));

				// Test SetCell
				AssertExtensions.ThrowsException<BoardException>(() => board.SetCell(0, 0, 9));
			}
        }

		/// <summary>
		/// Tests resetting cells.
		/// </summary>
		[TestMethod]
		public void TestResetCell()
		{
			using (var board = (Board)BoardSampleData.sampleBoard)
			{
				for (var i = 0; i < BoardSampleData.sampleBoard.Length; i++)
				{
					board.ResetCell(i / 9, i % 9);
				}

				Assert.AreEqual(0, ((byte[])board).Count(v => v != 0));
			}
		}

        [TestMethod]
        public void TestGetCell()
        {
			using (var board = (Board)BoardSampleData.sampleBoard)
            {
                Assert.AreEqual(0, board.GetCell(0, 0));
                Assert.AreEqual(0, board.GetCell(3, 3));
                Assert.AreEqual(0, board.GetCell(8, 8));

                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.GetCell(10, 0));
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.GetCell(0, 10));
            }
        }

        [TestMethod]
        public void TestGetterSetter()
        {
            using (var board = new Board())
            {
                for (int i = 0, rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < 9; columnIndex++)
                    {
						if (BoardSampleData.sampleBoard[i] != 0)
                        {
							board[rowIndex][columnIndex] = BoardSampleData.sampleBoard[i];
                        }

                        i++;
                    }
                }

                for (int i = 0, rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < 9; columnIndex++)
                    {
						Assert.AreEqual(board[rowIndex][columnIndex], BoardSampleData.sampleBoard[i++]);
                    }
                }

                var sampleBoardIndex = 0;
                foreach (var row in board.Rows)
                {
                    foreach (var cell in row)
                    {
						Assert.AreEqual(BoardSampleData.sampleBoard[sampleBoardIndex++], cell);
                    }
                }

                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board[9][0] = 1);
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board[0][9] = 1);
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board[0][0] = 10);
            }
        }

		[TestMethod]
		public void TestFromBytes()
		{
		}
    }
}

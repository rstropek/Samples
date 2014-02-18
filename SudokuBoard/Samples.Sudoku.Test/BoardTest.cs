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
        private static readonly byte[] sampleBoard = new byte[] {
                0, 2, 3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8, 9, 1, 2, 3,
                7, 8, 9, 1, 2, 3, 4, 5, 6,
                2, 3, 4, 0, 6, 7, 8, 9, 1,
                5, 6, 7, 8, 9, 1, 2, 3, 4,
                8, 9, 1, 2, 3, 4, 5, 6, 7,
                3, 4, 5, 6, 7, 8, 9, 1, 2,
                6, 7, 8, 9, 1, 2, 3, 4, 5,
                9, 1, 2, 3, 4, 5, 6, 7, 0 };

        [TestMethod]
        public void TestBoardTypeCasts()
        {
			// Test cast to byte[]
            using (var board = new Board())
            {
				var po = new PrivateObject(board);
				po.Invoke("SetBoardContentUnchecked", sampleBoard);

				Assert.IsTrue(((byte[])board).SequenceEqual(sampleBoard));
            }

			// Test cast from byte[] to Board
			using (var board = (Board)sampleBoard)
			{
				Assert.IsTrue(((byte[])board).SequenceEqual(sampleBoard));
			}
        }

		[TestMethod]
		public void TestCopyOfRow()
		{
			using (var board = new Board())
			{
				var po = new PrivateObject(board);
				po.Invoke("SetBoardContentUnchecked", sampleBoard);

				// Test copying first row
				var row = (byte[])board[0];
				Assert.IsTrue(row.SequenceEqual(sampleBoard.Take(9)));

				// Test copying a row in the middle
				row = (byte[])board[1];
				Assert.IsTrue(row.SequenceEqual(sampleBoard.Skip(9).Take(9)));

				// Test copying the last row
				row = (byte[])board[8];
				Assert.IsTrue(row.SequenceEqual(sampleBoard.Skip(8 * 9).Take(9)));

				// Test if argument is validated
				AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => { var r = (byte[])board[9]; });
			}
		}

        [TestMethod]
        public async Task TestIsValuePossible()
        {
			var pt = new PrivateType(typeof(Board));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInRow", BoardTest.sampleBoard, 8, (byte)9));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInColumn", BoardTest.sampleBoard, 8, (byte)9));

			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 0, 0, (byte)1));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 0, 0, (byte)2));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 3, 3, (byte)5));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 3, 3, (byte)6));
			Assert.IsTrue((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 8, 8, (byte)8));
			Assert.IsFalse((bool)pt.InvokeStatic("IsValuePossibleInSquare", BoardTest.sampleBoard, 8, 8, (byte)9));

			Assert.IsTrue(await (Task<bool>)pt.InvokeStatic("IsValuePossibleAsync", BoardTest.sampleBoard, 0, 0, (byte)1));
			Assert.IsFalse(await (Task<bool>)pt.InvokeStatic("IsValuePossibleAsync", BoardTest.sampleBoard, 0, 0, (byte)2));
        }

        [TestMethod]
        public void TestSetCell()
        {
            using (var board = new Board())
            {
                for (var i = 0; i < sampleBoard.Length; i++)
                {
                    if (sampleBoard[i] != 0)
                    {
                        Assert.IsTrue(board.TrySetCell(i / 9, i % 9, sampleBoard[i]));
                    }
                }

                for (int row = 0; row < 9; row++)
                {
                    Assert.IsTrue(board.GetCopyOfRow(row).SequenceEqual(sampleBoard.Skip(row * 9).Take(9)));
                }

                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(9, 0, 1));
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(0, 9, 1));
                AssertExtensions.ThrowsException<ArgumentOutOfRangeException>(() => board.TrySetCell(0, 0, 10));
            }
        }

        [TestMethod]
        public void TestGetCell()
        {
            using (var board = new Board())
            {
				var po = new PrivateObject(board);
				po.Invoke("SetBoardContentUnchecked", sampleBoard);

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
                        if (sampleBoard[i] != 0)
                        {
                            board[rowIndex][columnIndex] = sampleBoard[i];
                        }

                        i++;
                    }
                }

                for (int i = 0, rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < 9; columnIndex++)
                    {
                        Assert.AreEqual(board[rowIndex][columnIndex], sampleBoard[i++]);
                    }
                }

                var sampleBoardIndex = 0;
                foreach (var row in board.Rows)
                {
                    foreach (var cell in row)
                    {
                        Assert.AreEqual(sampleBoard[sampleBoardIndex++], cell);
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

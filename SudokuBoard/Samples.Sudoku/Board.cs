namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Linq;

	/// <summary>
	/// Represents a Sudoku board
	/// </summary>
	public class Board : IDisposable
	{
		#region Private fields
		// Note that fields that are initialized during object creation are marked
		// with "readonly".

		private readonly ReaderWriterLockSlim contentLock = new ReaderWriterLockSlim();
		private readonly byte[] content = new byte[9 * 9];
		private readonly BoardRow[] rows = new BoardRow[9];
		private bool disposed = false;
		#endregion

		public Board()
		{
			// Create objects representing board rows
			for (var row = 0; row < 9; row++)
			{
				// Note that the "localRow" variable is necessary for the closure to
				// work correctly.
				var localRow = row;

				// A board row does not store its own state. Therefore we have to provide
				// functions that the board row can use to read/write state.
				this.rows[row] = new BoardRow(
					zeroBasedColumnIndex => this.GetCell(localRow, zeroBasedColumnIndex),
					(zeroBasedColumnIndex, value) => this.SetCell(localRow, zeroBasedColumnIndex, value),
					() => this.GetCopyOfRow(localRow));
			}
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Technical error message, no need to translate it")]
		public static Board FromBytes(byte[] source)
		{
			ContractExtensions.IsNotNull(source, "source");
			ContractExtensions.IsArgumentInRange(source.Length == 9 * 9, "source", "Source does not have correct size (9 * 9 = 81 elements)");
			if (Contract.ForAll(source, cellValue => cellValue < 0 || cellValue > 9))
			{
				throw new ArgumentException("Source contains invalid values");
			}

			Contract.EndContractBlock();

			var result = new Board();
			try
			{
				for (byte rowIndex = 0, index = 0; rowIndex < 9; rowIndex++)
				{
					for (byte columnIndex = 0; columnIndex < 9; columnIndex++, index++)
					{
						// Store content of source[index] in a variable because it is accessed multiple times.
						var cellValue = source[index];

						// Ignore zero
						if (cellValue != 0)
						{
							if (!Board.TrySetCellInternal(result.content, rowIndex, columnIndex, cellValue))
							{
								throw new BoardException(string.Format(
									CultureInfo.InvariantCulture,
									"Specified source contains invalid value {0} in cell (row {1}, column {2})",
									cellValue,
									rowIndex,
									columnIndex));
							}
						}
					}
				}
			}
			catch
			{
				// Dispose created board in case of an exception
				result.Dispose();
				throw;
			}

			return result;
		}

		public static byte[] ToBytes(Board source)
		{
			ContractExtensions.IsNotNull(source, "source");
			ContractExtensions.IsNotDisposed(source, source.IsDisposed);
			Contract.EndContractBlock();

			var result = new byte[9 * 9];
			source.contentLock.EnterReadLock();
			try
			{
				source.content.CopyTo(result, 0);
				return result;
			}
			finally
			{
				source.contentLock.ExitReadLock();
			}

		}

		public static explicit operator Board(byte[] source)
		{
			return Board.FromBytes(source);
		}

		public static explicit operator byte[](Board source)
		{
			return Board.ToBytes(source);
		}

		~Board()
		{
			this.Dispose(false);
		}

		public IEnumerable<BoardRow> Rows
		{
			get
			{
				ContractExtensions.IsNotDisposed(this, this.IsDisposed);
				Contract.EndContractBlock();

				for (var row = 0; row < 9; row++ )
				{
					yield return this.rows[row];
				}
			}
		}

		public BoardRow this[int zeroBasedRowIndex]
		{
			get
			{
				ContractExtensions.IsNotDisposed(this, this.IsDisposed);
				ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
				Contract.EndContractBlock();

				return this.rows[zeroBasedRowIndex];
			}
		}

		public bool TrySetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
			ContractExtensions.IsValidValue(value, "value");
			Contract.EndContractBlock();

			this.contentLock.EnterWriteLock();
			try
			{
				if (Board.IsValuePossible(this.content, zeroBasedRowIndex, zeroBasedColumnIndex, value))
				{
					this.content[Board.CalculateIndexFromRowAndColumn(zeroBasedRowIndex, zeroBasedColumnIndex)] = value;
					return true;
				}

				return false;
			}
			finally
			{
				this.contentLock.ExitWriteLock();
			}
		}

		public void SetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			if (!this.TrySetCell(zeroBasedRowIndex, zeroBasedColumnIndex, value))
			{
				throw new BoardException("Specified value is not possible at the specified position");
			}
		}

		public byte GetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
			Contract.EndContractBlock();

			this.contentLock.EnterReadLock();
			try
			{
				return this.content[Board.CalculateIndexFromRowAndColumn(zeroBasedRowIndex, zeroBasedColumnIndex)];
			}
			finally
			{
				this.contentLock.ExitReadLock();
			}
		}

		public void ResetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
			Contract.EndContractBlock();

			this.contentLock.EnterWriteLock();
			try
			{
				this.content[Board.CalculateIndexFromRowAndColumn(zeroBasedRowIndex, zeroBasedColumnIndex)] = 0;
			}
			finally
			{
				this.contentLock.ExitWriteLock();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Cannot overflow because index is small")]
		public byte[] GetCopyOfRow(int zeroBasedRowIndex)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			Contract.EndContractBlock();

			var result = new byte[9];
			this.contentLock.EnterReadLock();
			try
			{
				Array.Copy(this.content, zeroBasedRowIndex * 9, result, 0, 9);
			}
			finally
			{
				this.contentLock.ExitReadLock();
			}

			return result;
		}

		#region IDisposable implementation
		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		protected virtual void Dispose(bool disposeObject)
		{
			if (disposeObject)
			{
				this.contentLock.Dispose();
				this.disposed = true;
			}
		}
		#endregion

		#region Methods for validating board values
		private static bool IsValuePossible(byte[] boardData, int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			return Board.IsValuePossibleAsync(boardData, zeroBasedRowIndex, zeroBasedColumnIndex, value).Result;
		}

		private static Task<bool> IsValuePossibleAsync(byte[] boardData, int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			Board.BoardDataContract(boardData);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.CellValueContract(value);

			var tasks = new Task<bool>[3];
			tasks[0] = Task.Run<bool>(() => Board.IsValuePossibleInRow(boardData, zeroBasedRowIndex, value));
			tasks[1] = Task.Run<bool>(() => Board.IsValuePossibleInColumn(boardData, zeroBasedColumnIndex, value));
			tasks[2] = Task.Run<bool>(() => Board.IsValuePossibleInSquare(boardData, zeroBasedRowIndex, zeroBasedColumnIndex, value));
			return Task.Factory.ContinueWhenAll(tasks, t => t[0].Result && t[1].Result && t[2].Result);
		}

		private static bool IsValuePossibleInRow(byte[] boardData, int zeroBasedRowIndex, byte value)
		{
			Board.BoardDataContract(boardData);
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.CellValueContract(value);

			for (int i = 0, index = zeroBasedRowIndex * 9; i < 9; i++, index++)
			{
				if (boardData[index] == value)
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsValuePossibleInColumn(byte[] boardData, int zeroBasedColumnIndex, byte value)
		{
			Board.BoardDataContract(boardData);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Board.CellValueContract(value);

			for (int i = 0, index = zeroBasedColumnIndex; i < 9; i++, index += 9)
			{
				if (boardData[index] == value)
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsValuePossibleInSquare(byte[] boardData, int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			Board.BoardDataContract(boardData);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.CellValueContract(value);

			for (int r = 0, rowIndex = (zeroBasedRowIndex / 3) * 3; r < 3; r++, rowIndex++)
			{
				for (int c = 0, columnIndex = (zeroBasedColumnIndex / 3) * 3; c < 3; c++, columnIndex++)
				{
					if (boardData[(rowIndex * 9) + columnIndex] == value)
					{
						return false;
					}
				}
			}

			return true;
		}
		#endregion

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Helper method used by unit tests")]
		private void SetBoardContentUnchecked(byte[] boardContent)
		{
			this.contentLock.EnterWriteLock();
			try
			{
				boardContent.CopyTo(this.content, 0);
			}
			finally
			{
				this.contentLock.ExitWriteLock();
			}
		}

		private static bool TrySetCellInternal(byte[] boardData, int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			Board.BoardDataContract(boardData);
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Board.CellValueContract(value, allowZero: false);

			if (Board.IsValuePossible(boardData, zeroBasedRowIndex, zeroBasedColumnIndex, value))
			{
				boardData[Board.CalculateIndexFromRowAndColumn(zeroBasedRowIndex, zeroBasedColumnIndex)] = value;
				return true;
			}

			return false;
		}

		private static int CalculateIndexFromRowAndColumn(int zeroBasedRowIndex, int zeroBasedColumnIndex)
		{
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Contract.Ensures(Contract.Result<int>() < 9 * 9);

			return zeroBasedRowIndex * 9 + zeroBasedColumnIndex;
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by code contracts")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Used by code contracts")]
		[ContractAbbreviator]
		private static void IndexValidContract(int zeroBasedIndex)
		{
			Contract.Requires(zeroBasedIndex >= 0);
			Contract.Requires(zeroBasedIndex < 9);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by code contracts")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Used by code contracts")]
		[ContractAbbreviator]
		private static void BoardDataContract(byte[] boardData)
		{
			Contract.Requires(boardData != null);
			Contract.Requires(boardData.Length == 9 * 9);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by code contracts")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Used by code contracts")]
		[ContractAbbreviator]
		private static void CellValueContract(byte value, bool allowZero = false)
		{
			Contract.Requires(value >= 1);
			Contract.Requires(value <= 9);
		}
	}
}
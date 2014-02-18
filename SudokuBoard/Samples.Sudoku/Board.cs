namespace Samples.Sudoku
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents a Sudoku board
	/// </summary>
	public class Board : IDisposable
	{
		#region Private fields
		// Note that fields that are initialized during object creation are marked
		// with "readonly".

		/// <summary>
		/// Semaphore used to protect access to board data (<see cref="content"/>).
		/// </summary>
		private readonly ReaderWriterLockSlim contentLock = new ReaderWriterLockSlim();

		/// <summary>
		/// Board data.
		/// </summary>
		private readonly byte[] content = new byte[9 * 9];

		/// <summary>
		/// Helper objects used to access the rows of the Sudoku board.
		/// </summary>
		private readonly BoardRow[] rows = null;

		/// <summary>
		/// Value indicating whether the object has been disposed.
		/// </summary>
		private bool disposed = false;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Board"/> class.
		/// </summary>
		public Board()
		{
			// Create objects representing board rows. Note the use of Linq.
			this.rows = Enumerable.Range(0, 9)
				.Select(row =>
				{
					// Helper variable; necessary for closure to work correctly.
					var internalRow = row;

					// Note the use of lambdas and functional programming here.
					return new BoardRow(
						zeroBasedColumnIndex => this.GetCell(internalRow, zeroBasedColumnIndex),
						(zeroBasedColumnIndex, value) => this.SetCell(internalRow, zeroBasedColumnIndex, value),
						() => this.GetCopyOfRow(internalRow));
				})
				.ToArray();
		}

		#region Type cast operators
		/// <summary>
		/// Type cast from bytes to <see cref="Board"/> instance.
		/// </summary>
		/// <param name="source">Source byte array containing board data.</param>
		/// <returns>New <see cref="Board"/> instance.</returns>
		/// <remarks>
		/// Note the use of unit test as a code sample.
		/// </remarks>
		/// <example>
		/// <code source="../Samples.Sudoku.Test/BoardTest.cs" region="Example for type cast operators"
		///		language="C#" />
		/// </example>
		/// <seealso cref="FromBytes"/>
		public static explicit operator Board(byte[] source)
		{
			return Board.FromBytes(source);
		}

		/// <summary>
		/// Type cast from <see cref="Board"/> instance to bytes.
		/// </summary>
		/// <param name="source">Source board.</param>
		/// <returns>Board data as a byte array.</returns>
		/// <example>
		/// <code source="../Samples.Sudoku.Test/BoardTest.cs" region="Example for type cast operators"
		///		language="C#" />
		/// </example>
		/// <seealso cref="ToBytes"/>
		public static explicit operator byte[](Board source)
		{
			return Board.ToBytes(source);
		}

		/// <summary>
		/// Converts board bytes to <see cref="Board"/> instance.
		/// </summary>
		/// <param name="source">Source byte array containing board data.</param>
		/// <returns>New <see cref="Board"/> instance.</returns>
		/// <remarks>
		/// It is best practice to offer a FromXXX and ToXXX method if you create
		/// type cast operators. Note that if you forget, Visual Studio Code Analysis will warn you.
		/// You see, code analysis is a very good thing.
		/// </remarks>
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Technical error message, no need to translate it")]
		public static Board FromBytes(byte[] source)
		{
			// The following lines represent the contracts that are checked even in release builds.
			// Commonly used contracts were extracted into the ContractExtensions helper class.
			// Note the use of the ContractArgumentValidator attribute in the helper methods.

			// Note that you can set the 'Emit contracts into XML doc file' project option for 
			// code contracts. If you do so, Sandcastle will include the contract in the generated
			// documentation.

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
							// Note that it is not necessary to protect board bytes because result
							// has just been created and nobody else has a reference on it.
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

		/// <summary>
		/// Converts <see cref="Board"/> instance to bytes.
		/// </summary>
		/// <param name="source">Source board.</param>
		/// <returns>Board data as a byte array.</returns>
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
		#endregion

		#region Access to board data
		/// <summary>
		/// Tries the set the value of a cell.
		/// </summary>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
		/// <param name="zeroBasedColumnIndex">Index of the column.</param>
		/// <param name="value">The new value.</param>
		/// <returns><c>True</c> if the value could be set successfully, otherwise <c>false</c>.</returns>
		/// <remarks>
		/// <para>
		/// Note that <paramref name="value"/> must not be zero. If you want to unset a cell,
		/// use <see cref="ResetCell"/> instead.
		/// </para>
		/// <para>
		/// Note the TryXXX naming pattern. <see cref="TrySetCell"/> does the same as <see cref="SetCell"/>
		/// except that it does not throw an exception if the value could not be set.
		/// </para>
		/// </remarks>
		public bool TrySetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
			ContractExtensions.IsValidValue(value, "value");
			Contract.EndContractBlock();

			// Note that we use a write lock this time. This makes sure that only one thread
			// can write to the board bytes at the same time.
			this.contentLock.EnterWriteLock();
			try
			{
				// Call internal implementation.
				return Board.TrySetCellInternal(this.content, zeroBasedRowIndex, zeroBasedColumnIndex, value);
			}
			finally
			{
				this.contentLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Sets the value of a cell.
		/// </summary>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
		/// <param name="zeroBasedColumnIndex">Index of the column.</param>
		/// <param name="value">The new value.</param>
		/// <exception cref="BoardException">Thrown if value could not be set.</exception>
		/// <remarks>
		/// <para>
		/// Note that <paramref name="value"/> must not be zero. If you want to unset a cell,
		/// use <see cref="ResetCell"/> instead.
		/// </para>
		/// <para>
		/// Note the TryXXX naming pattern. <see cref="TrySetCell"/> does the same as <see cref="SetCell"/>
		/// except that it does not throw an exception if the value could not be set.
		/// </para>
		/// </remarks>
		public void SetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex, byte value)
		{
			// Note that we do not need contracts here because contracts are included in
			// the implementation of TrySetCell.

			if (!this.TrySetCell(zeroBasedRowIndex, zeroBasedColumnIndex, value))
			{
				throw new BoardException("Specified value is not possible at the specified position");
			}
		}

		/// <summary>
		/// Gets the value of a cell.
		/// </summary>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
		/// <param name="zeroBasedColumnIndex">Index of the column.</param>
		/// <returns>Value of the cell at the specified row and column index.</returns>
		/// <remarks>
		/// Zero is returned for an unset cell.
		/// </remarks>
		public byte GetCell(int zeroBasedRowIndex, int zeroBasedColumnIndex)
		{
			ContractExtensions.IsNotDisposed(this, this.IsDisposed);
			ContractExtensions.IsValidIndex(zeroBasedRowIndex, "zeroBasedRowIndex");
			ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
			Contract.EndContractBlock();

			// Note that we use a read lock this time. This makes sure that many threads
			// can read the board's content at the same time.
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

		/// <summary>
		/// Resets a cell's value to unset.
		/// </summary>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
		/// <param name="zeroBasedColumnIndex">Index of the column.</param>
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

		/// <summary>
		/// Internal implementation for setting a cell's value.
		/// </summary>
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
		#endregion

		#region Methods for validating board values
		// Note that the methods for validating board values are pure functions. They do not
		// depend on the rest of the Board class infrastructure. Therefore they are testing friendly.

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

			// Note how we run the three tests in parallel. They are independent of each other.
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

		#region Access to board rows
		/// <summary>
		/// Gets the rows of the board.
		/// </summary>
		/// <value>
		/// The rows or the board.
		/// </value>
		public IEnumerable<BoardRow> Rows
		{
			get
			{
				ContractExtensions.IsNotDisposed(this, this.IsDisposed);
				Contract.EndContractBlock();

				// Note the use of an enumerator block here.
				for (var row = 0; row < 9; row++)
				{
					yield return this.rows[row];
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="BoardRow"/> with the specified zero based row index.
		/// </summary>
		/// <value>
		/// The <see cref="BoardRow"/> at index <paramref name="zeroBasedRowIndex"/>.
		/// </value>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
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

		/// <summary>
		/// Gets a copy of a row as a byte array.
		/// </summary>
		/// <param name="zeroBasedRowIndex">Index of the row.</param>
		/// <returns>Bytes of the specified row.</returns>
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
		#endregion 

		#region IDisposable implementation
		/// <summary>
		/// Finalizes an instance of the <see cref="Board"/> class.
		/// </summary>
		~Board()
		{
			this.Dispose(false);
		}

		// Note the usage of inheritdoc instead of repeating documentation for the
		// Dispose method.

		/// <inheritdoc />
		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Gets a value indicating whether object is disposed.
		/// </summary>
		/// <value>
		///   <c>true</c> if object is disposed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposeObject"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposeObject)
		{
			if (disposeObject)
			{
				this.contentLock.Dispose();
				this.disposed = true;
			}
		}
		#endregion

		#region Various helper methods
		private static int CalculateIndexFromRowAndColumn(int zeroBasedRowIndex, int zeroBasedColumnIndex)
		{
			Board.IndexValidContract(zeroBasedRowIndex);
			Board.IndexValidContract(zeroBasedColumnIndex);
			Contract.Ensures(Contract.Result<int>() < 9 * 9);

			return zeroBasedRowIndex * 9 + zeroBasedColumnIndex;
		}

		// Note the use of the ContractAbbreviator attribute here. It is necessary
		// because the method only contains code contracts.
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
		#endregion
	}
}
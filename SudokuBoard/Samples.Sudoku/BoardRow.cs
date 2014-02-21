namespace Samples.Sudoku
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;

	/// <summary>
	/// Represents a row of a board.
	/// </summary>
	/// <seealso cref="Board"/>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name should be like this")]
    public class BoardRow : IEnumerable<byte>, IEnumerable
    {
        private Action<int, byte> setter;
        private Func<int, byte> getter;
        private Func<byte[]> copier;

        internal BoardRow(Func<int, byte> getter, Action<int, byte> setter, Func<byte[]> copier)
        {
			Contract.Requires(getter != null);
			Contract.Requires(setter != null);
			Contract.Requires(copier != null);

            this.getter = getter;
            this.setter = setter;
            this.copier = copier;
        }

		/// <summary>
		/// Gets or sets the value of the cell at the specified column index.
		/// </summary>
		/// <value>
		/// The value of the cell.
		/// </value>
		/// <param name="zeroBasedColumnIndex">Index of the column.</param>
        public byte this[int zeroBasedColumnIndex]
        {
            get
            {
				ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
                return this.getter(zeroBasedColumnIndex);
            }

            set
            {
				ContractExtensions.IsValidIndex(zeroBasedColumnIndex, "zeroBasedColumnIndex");
				this.setter(zeroBasedColumnIndex, value);
            }
        }

		/// <summary>
		/// Type cast from <see cref="BoardRow"/> instance to bytes.
		/// </summary>
		/// <param name="row">Source board row.</param>
		/// <returns>Board row data as a byte array.</returns>
		/// <seealso cref="ToBytes"/>
		public static implicit operator byte[](BoardRow row)
        {
			ContractExtensions.IsNotNull(row, "row");
			return row.ToBytes();
        }

		/// <summary>
		/// Converts <see cref="BoardRow"/> instance to bytes.
		/// </summary>
		/// <returns>Board row data as a byte array.</returns>
		public byte[] ToBytes()
        {
			// Note the use of a postcondition here.
			Contract.Ensures(Contract.Result<byte[]>() != null);
			Contract.Ensures(Contract.Result<byte[]>().Length == 9);
			return this.copier();
        }

		/// <inheritdoc />
        public IEnumerator<byte> GetEnumerator()
        {
            return this.ToBytes().AsEnumerable<byte>().GetEnumerator();
        }

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ToBytes().GetEnumerator();
        }
    }
}

namespace Samples.Sudoku
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Threading.Tasks;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name should be like this")]
    public class BoardRow : IEnumerable<byte>
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

        public byte this[int zeroBasedColumnIndex]
        {
            get
            {
				Contract.Requires(zeroBasedColumnIndex >= 0 && zeroBasedColumnIndex < 9);
                return this.getter(zeroBasedColumnIndex);
            }

            set
            {
				Contract.Requires(zeroBasedColumnIndex >= 0 && zeroBasedColumnIndex < 9);
				this.setter(zeroBasedColumnIndex, value);
            }
        }

        public static implicit operator byte[](BoardRow row)
        {
			Contract.Requires(row != null);
			return row.ToBytes();
        }

        public byte[] ToBytes()
        {
			Contract.Ensures(Contract.Result<byte[]>() != null);
			Contract.Ensures(Contract.Result<byte[]>().Length == 9);
			return this.copier();
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return this.ToBytes().AsEnumerable<byte>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ToBytes().GetEnumerator();
        }
    }
}

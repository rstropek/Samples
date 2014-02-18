namespace Samples.Sudoku
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Contains helper methods for commonly used release contracts.
	/// </summary>
	internal static class ContractExtensions
	{
		[ContractArgumentValidator]
		public static void IsNotNull([ValidatedNotNull] object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}

			Contract.EndContractBlock();
		}

		[ContractArgumentValidator]
		public static void IsArgumentInRange(bool isInRange, string argumentName, string message)
		{
			if (!isInRange)
			{
				throw new ArgumentOutOfRangeException(argumentName, message);
			}

			Contract.EndContractBlock();
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Technical error message, no need to translate it")]
		[ContractArgumentValidator]
		public static void IsValidIndex(int zeroBasedIndex, string argumentName)
		{
			ContractExtensions.IsArgumentInRange(zeroBasedIndex >= 0, argumentName, "Index has to be >= 0");
			ContractExtensions.IsArgumentInRange(zeroBasedIndex < 9, argumentName, "Index has to be < 9");

			Contract.EndContractBlock();
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Technical error message, no need to translate it")]
		[ContractArgumentValidator]
		public static void IsValidValue(int value, string argumentName)
		{
			ContractExtensions.IsArgumentInRange(value > 0, argumentName, "Value has to be > 0");
			ContractExtensions.IsArgumentInRange(value <= 9, argumentName, "Value has to be <= 9");

			Contract.EndContractBlock();
		}

		[ContractArgumentValidator]
		public static void IsNotDisposed(object disposableObject, bool disposed)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(disposableObject.ToString());
			}

			Contract.EndContractBlock();
		}
	}
}

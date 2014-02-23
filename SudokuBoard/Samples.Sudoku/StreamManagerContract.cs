namespace Samples.Sudoku
{
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading.Tasks;

	// Note how we add a contract the the IStreamManager interface using the
	// ContractClassFor attribute.

	[ContractClassFor(typeof(IStreamManager))]
	internal abstract class StreamManagerContract : IStreamManager
	{
		public Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode)
		{
			// Note that we cannot make this precondition a legacy-requires in our sample.
			Contract.Requires(boardName != null);

			// The following postcondition checks the resulting task.
			Contract.Ensures(Contract.Result<Task<Stream>>() != null);

			// The following postcondition checks the result once the Task has been completed.
			Contract.Ensures(Contract.Result<Task<Stream>>().Result != null);

			return default(Task<Stream>);
		}
	}
}

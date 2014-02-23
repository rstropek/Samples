namespace Samples.Sudoku
{
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading.Tasks;

	// Note how we add a contract to this interface using the ContractClass attribute.

	/// <summary>
	/// Contains methods to manage a stream for reading/writing board data.
	/// </summary>
	[ContractClass(typeof(StreamManagerContract))]
	public interface IStreamManager
	{
		/// <summary>
		/// Opens the stream asynchronously.
		/// </summary>
		/// <param name="boardName">Name of the board to access.</param>
		/// <param name="accessMode">The <see cref="AccessMode"/>.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The value of the 
		/// TResult parameter contains the resulting stream.
		/// </returns>
		/// <remarks>
		/// Note that the caller is responsible for disposing the returned stream when
		/// it is no longer needed.
		/// </remarks>
		Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode);
	}
}

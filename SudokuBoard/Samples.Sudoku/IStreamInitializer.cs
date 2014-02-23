namespace Samples.Sudoku
{
	using System.IO;
	using System.Threading.Tasks;

	/// <summary>
	/// Contains methods to open a stream for reading/writing board data.
	/// </summary>
	public interface IStreamInitializer
	{
		/// <summary>
		/// Opens the stream asynchronously.
		/// </summary>
		/// <param name="boardName">Name of the board.</param>
		/// <param name="accessMode">The access mode.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The value of the 
		/// TResult parameter contains the open stream.
		/// </returns>
		/// <remarks>
		/// Note that the caller is responsible for disposing the returned stream when
		/// it is no longer needed.
		/// </remarks>
		Task<Stream> OpenStreamAsync(string boardName, AccessMode accessMode);
	}
}

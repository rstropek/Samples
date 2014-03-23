// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="TravelType.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Snek.ToolsSample.Logic
{
	/// <summary>
	/// Represents a type of travel.
	/// </summary>
	public enum TravelType
	{
		/// <summary>
		/// The arrival at the customer.
		/// </summary>
		Arrival = 0,

		/// <summary>
		/// Traveling from one customer to the other.
		/// </summary>
		Continuation = 1,

		/// <summary>
		/// The homeward journey.
		/// </summary>
		Homeward = 2
	}
}

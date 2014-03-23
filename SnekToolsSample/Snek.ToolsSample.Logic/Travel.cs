// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="Travel.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Snek.ToolsSample.Logic
{
	using System;

	/// <summary>
	/// Represents a business travel with associated costs.
	/// </summary>
	public class Travel
	{
		/// <summary>
		/// Gets the travel date.
		/// </summary>
		public DateTime TravelDate { get; internal set; }

		/// <summary>
		/// Gets the per diems cost.
		/// </summary>
		public decimal PerDiemsCost { get; internal set; }
	}
}

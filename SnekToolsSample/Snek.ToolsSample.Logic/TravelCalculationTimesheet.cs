// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="TravelCalculationTimesheet.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Snek.ToolsSample.Logic
{
	using System;

	/// <summary>
	/// Represents a timesheet record used for travel calculation.
	/// </summary>
	public class TravelCalculationTimesheet
	{
		/// <summary>
		/// Gets or sets the begin time.
		/// </summary>
		public DateTime BeginTime { get; set; }

		/// <summary>
		/// Gets or sets the end time.
		/// </summary>
		public DateTime EndTime { get; set; }

		/// <summary>
		/// Gets or sets the type of the travel.
		/// </summary>
		public TravelType TravelType { get; set; }
	}
}

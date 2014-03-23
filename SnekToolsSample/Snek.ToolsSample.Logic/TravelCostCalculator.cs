// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="TravelCostCalculator.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Snek.ToolsSample.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;

	internal static class TravelCostCalculator
	{
		public static IEnumerable<TravelCalculationTimesheet> GetTravelCalculationTimesheets(DataTable timesheets)
		{
			return
				timesheets.Rows.Cast<DataRow>()
					.Select(
						timesheet => new TravelCalculationTimesheet()
										{
											// Note that this conversion is inefficient. Demo how
											// we find and solve this problem with a profiler during the
											// session.
											BeginTime = DateTime.Parse(timesheet[0].ToString()),
											EndTime = DateTime.Parse(timesheet[1].ToString()),
											TravelType = (TravelType)((short)timesheet[2])
										});
		}

		public static IEnumerable<Travel> FindAndProcessTravels(IEnumerable<TravelCalculationTimesheet> timesheets)
		{
			// travelElements contains all details of the travel. The first item in 
			// travelElements is the arrival, followed by travel 
			// continuations. The last item in travel Elements is the homeward journey.
			var travelElements = new List<TravelCalculationTimesheet>();

			// Result
			var result = new List<Travel>();

			// Flags indicating whether start and end of travel have been found.
			var foundStartTimesheet = false;
			var foundEndTimesheet = false;
			foreach (var travel in timesheets)
			{
				if (travel.TravelType == TravelType.Arrival)
				{
					// Arrival
					if (foundStartTimesheet)
					{
						// Ignore start of travel because there is no end of travel
						travelElements.Clear();
					}
					else
					{
						foundStartTimesheet = true;
					}
				}
				else if (travel.TravelType == TravelType.Homeward)
				{
					// Homeward journey
					if (!foundStartTimesheet)
					{
						// Ignore homeward journey because no arrival could be found
						continue;
					}

					foundEndTimesheet = true;
				}
				else
				{
					// Continuous travel
					if (!foundStartTimesheet)
					{
						// Ignore continuation because no arrival could be found
						continue;
					}
				}

				travelElements.Add(travel);

				if (foundEndTimesheet)
				{
					result.AddRange(ProcessTravelTimesheets(travelElements));
					travelElements.Clear();
					foundStartTimesheet = foundEndTimesheet = false;
				}
			}

			return result;
		}

		private static IEnumerable<Travel> ProcessTravelTimesheets(List<TravelCalculationTimesheet> travelElements)
		{
			// Enumeration receiving the resulting TravelCosts objects
			var result = new List<Travel>();

			if (travelElements.Count == 0)
			{
				return result;
			}

			// Ignore travels with less than 8h
			if ((travelElements[travelElements.Count - 1].EndTime - travelElements[0].BeginTime).TotalHours < 8)
			{
				return result;
			}

			// Zero-based index of current item in travelElements
			var currentIndex = 0;

			// Variable used to iterate over 24h periods
			var fullDayIterator = travelElements[0].BeginTime;

			var singleDayTrip = travelElements[currentIndex].BeginTime.Date
								  == travelElements[travelElements.Count - 1].BeginTime.Date;

			// Start with begin time of travel and iterate in 24h intervals until
			//   travel end time of last travel element has been reached
			while (fullDayIterator < travelElements[travelElements.Count - 1].EndTime)
			{
				// Read over all travel elements of the current day
				while ((fullDayIterator.Date == travelElements[currentIndex + 1].BeginTime.Date)
					   && travelElements[currentIndex + 1].TravelType != TravelType.Homeward)
				{
					currentIndex++;
				}

				if (fullDayIterator.Date == travelElements[0].BeginTime.Date && !singleDayTrip)
				{
					// Arrival day
					result.Add(new Travel() { TravelDate = fullDayIterator.Date, PerDiemsCost = 12.0m });
				}
				else if (fullDayIterator.Date == travelElements[travelElements.Count - 1].BeginTime.Date || singleDayTrip)
				{
					// Homeward Journey
					result.Add(new Travel() { TravelDate = fullDayIterator.Date, PerDiemsCost = 12.0m });
				}
				else
				{
					// Intermediate day
					result.Add(new Travel() { TravelDate = fullDayIterator.Date, PerDiemsCost = 24.0m });
				}

				// Move to next 24h interval
				fullDayIterator = fullDayIterator.AddHours(24.0);
			}

			return result;
		}
	}
}

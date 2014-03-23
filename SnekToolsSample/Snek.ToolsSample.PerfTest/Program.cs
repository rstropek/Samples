namespace Snek.ToolsSample.PerfTest
{
	using System;
	using System.Data;
	using System.Diagnostics;

	using Snek.ToolsSample.Logic;

	public static class Program
	{
		public static void Main(string[] args)
		{
			// This sample program triggers a very large amount of travel calculations.
			// It is used as a reference workload for optimizing the travel cost calculation
			// algorithm.

			// Prepare (in real life, the table would be read from a database)
			var sourceTable = GenerateLargeInputTable();

			var watch = new Stopwatch();
			watch.Start();

			// Translate data table into objects
			var sourceTimesheets = TravelCostCalculator.GetTravelCalculationTimesheets(sourceTable);

			// Process travel timesheets
			var result = TravelCostCalculator.FindAndProcessTravels(sourceTimesheets);

			watch.Stop();
			Console.WriteLine(watch.Elapsed);
		}

		private static DataTable GenerateLargeInputTable()
		{
			var sourceTable = new DataTable();
			sourceTable.Columns.Add("BeginTime", typeof(DateTime));
			sourceTable.Columns.Add("EndTime", typeof(DateTime));
			sourceTable.Columns.Add("TravelTypeId", typeof(short));

			for (var date = new DateTime(1000, 1, 1); date.Year <= 3000; date = date.AddDays(3))
			{
				sourceTable.LoadDataRow(new object[] { date.AddHours(8.0), date.AddHours(10.0), (short)0 }, LoadOption.OverwriteChanges);
				sourceTable.LoadDataRow(new object[] { date.AddHours(17.0), date.AddHours(19.0), (short)1 }, LoadOption.OverwriteChanges);
				sourceTable.LoadDataRow(new object[] { date.AddHours((24.0 * 2) + 17.0), date.AddHours((24.0 * 2) + 19.0), (short)2 }, LoadOption.OverwriteChanges);
			}

			return sourceTable;
		}
	}
}

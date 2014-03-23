// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="TimesheetDatabaseContext.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Snek.ToolsSample.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;

	/// <summary>
	/// Represents a database connection to a Timesheet database.
	/// </summary>
	public class TimesheetDatabaseContext : IDisposable
	{
		private SqlConnection connection = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimesheetDatabaseContext"/> class.
		/// </summary>
		/// <param name="configurationConnectionStringName">
		/// Name of the configuration setting containing the connection string.
		/// </param>
		/// <remarks>
		/// If you do not specify <paramref name="configurationConnectionStringName"/>,
		/// the method will look for a connection string setting named TimesheetDatabase.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed, is OK.")]
		public TimesheetDatabaseContext(string configurationConnectionStringName = null)
		{
			if (configurationConnectionStringName == null)
			{
				configurationConnectionStringName = "TimesheetDatabase";
			}

			var connectionString = ConfigurationManager.ConnectionStrings[configurationConnectionStringName];
			if (connectionString == null)
			{
				throw new ArgumentException("Could not find connection string in configuration file", "configurationConnectionStringName");
			}

			if (!string.IsNullOrEmpty(connectionString.ProviderName) && connectionString.ProviderName != "System.Data.SqlClient")
			{
				throw new ArgumentException("Invalid provider name in configuration file. Expected System.Data.SqlClient", "configurationConnectionStringName");
			}

			this.connection = new SqlConnection(connectionString.ConnectionString);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="TimesheetDatabaseContext"/> class.
		/// </summary>
		~TimesheetDatabaseContext()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Generates the demo data.
		/// </summary>
		public void GenerateDemoData()
		{
			this.Open();
			SampleDataGenerator.GenerateSampleData(this.connection);
		}

		/// <summary>
		/// Finds the and processes travels.
		/// </summary>
		/// <returns>List of travels with their associated costs.</returns>
		public IEnumerable<Travel> FindAndProcessTravels()
		{
			using (var timesheets = this.GetTravelTimesheetsDataTable())
			{
				return TravelCostCalculator.FindAndProcessTravels(
					TravelCostCalculator.GetTravelCalculationTimesheets(timesheets));
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller has to dispose")]
		internal DataTable GetTravelTimesheetsDataTable()
		{
			this.Open();

			using (var cmd = this.connection.CreateCommand())
			{
				cmd.CommandText = "SELECT BeginTime, EndTime, TravelTypeId FROM Timesheet WHERE TravelTypeId IS NOT NULL ORDER BY BeginTime";
				var result = new DataTable() { Locale = CultureInfo.InvariantCulture };
				using (var reader = cmd.ExecuteReader())
				{
					result.Load(reader);
				}

				return result;
			}
		}

		private void Open()
		{
			// Open database if not already open
			if (this.connection.State != ConnectionState.Open)
			{
				this.connection.Open();
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing && this.connection != null)
			{
				this.connection.Dispose();
				this.connection = null;
			}
		}
	}
}

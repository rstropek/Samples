// --------------------------------------------------------------------------------------------------------------------
// <copyright company="software architects gmbh" file="TimesheetDatabaseContext.cs">
//   (c) software architects gmbh
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Snek.ToolsSample.Logic
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// Represents a database connection to a Timesheet database.
	/// </summary>
	public class TimesheetDatabaseContext : IDisposable
	{
		/// <summary>
		/// Underlying database connection.
		/// </summary>
		private SqlConnection connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimesheetDatabaseContext" /> class.
		/// </summary>
		/// <param name="configurationConnectionStringName">Name of the configuration setting containing the connection string.</param>
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
		/// <exception cref="System.InvalidOperationException">Sample data file not found.</exception>
		public void GenerateDemoData()
		{
			// Open database if not already open
			if (this.connection.State != ConnectionState.Open)
			{
				this.connection.Open();
			}

			SampleDataGenerator.GenerateSampleData(this.connection);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
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

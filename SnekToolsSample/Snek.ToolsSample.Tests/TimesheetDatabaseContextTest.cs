namespace Snek.ToolsSample.Tests
{
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Snek.ToolsSample.Logic;
	using System;
	using System.Configuration;
	using System.Configuration.Fakes;

	[TestClass]
	public class TimesheetDatabaseContextTest
	{
		[TestMethod]
		public void TestTimesheetDatabaseContextConstructor()
		{
			using (ShimsContext.Create())
			{
				var settingName = string.Empty;
				string providerName = null;
				ShimConfigurationManager.ConnectionStringsGet =
					() => new ConnectionStringSettingsCollection()
						      {
							      // ReSharper disable once AccessToModifiedClosure
							      new ConnectionStringSettings(
								      settingName,
								      "Server=DummyServer;Database=DummyDatabase;Integrated Security=true",
								      // ReSharper disable once ExpressionIsAlwaysNull
								      // ReSharper disable once AccessToModifiedClosure
								      providerName)
						      };

				// Test if constructor with empty parameter list works
				settingName = "TimesheetDatabase";
				providerName = null;
				new TimesheetDatabaseContext().Dispose();

				// Test if constructor with settings name works
				settingName = "MySettingName";
				providerName = null;
				new TimesheetDatabaseContext("MySettingName").Dispose();

				// Test if invalid provider name is detected
				settingName = "TimesheetDatabase";
				providerName = "DummyProvider";
				ExpectException<ArgumentException>(
					() => new TimesheetDatabaseContext().Dispose(),
					ex => ex.ParamName == "configurationConnectionStringName");

				// Test if missing setting is detected
				settingName = "TimesheetDatabase";
				providerName = null;
				ExpectException<ArgumentException>(
					() => new TimesheetDatabaseContext("Dummy").Dispose(),
					ex => ex.ParamName == "configurationConnectionStringName");
			}
		}

		private static void ExpectException<T>(Action body, Func<T, bool> exceptionValidator = null) where T : Exception
		{
			var exceptionWasThrown = false;
			try
			{
				body();
			}
			catch (T ex)
			{
				if (exceptionValidator != null)
				{
					Assert.IsTrue(exceptionValidator(ex));
				}

				exceptionWasThrown = true;
			}

			Assert.IsTrue(exceptionWasThrown);
		}
	}
}

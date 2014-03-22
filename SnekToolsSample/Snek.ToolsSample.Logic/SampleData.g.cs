namespace Snek.ToolsSample.Logic
{
	using System.Data;
	using System.Data.SqlClient;

	internal static class SampleDataGenerator
	{
		internal static void GenerateSampleData(SqlConnection conn)
		{
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText = @"
BEGIN TRANSACTION;

-- Cleanup Timesheet table before inserting demo data
DELETE FROM Timesheet;

-- Now we can generate demo data
INSERT INTO Timesheet ( BeginTime, EndTime, Description, TravelTypeId )
	VALUES ( '2014-01-01T08:00:00', '2014-01-01T10:00:00', 'Hinreise', 0 );
INSERT INTO Timesheet ( BeginTime, EndTime, Description, TravelTypeId )
	VALUES ( '2014-01-01T17:00:00', '2014-01-01T19:00:00', 'Rückreise', 2 );
INSERT INTO Timesheet ( BeginTime, EndTime, Description, TravelTypeId )
	VALUES ( '2014-01-02T08:00:00', '2014-01-02T10:00:00', 'Hinreise', 0 );
INSERT INTO Timesheet ( BeginTime, EndTime, Description, TravelTypeId )
	VALUES ( '2014-01-02T16:00:00', '2014-01-02T18:00:00', 'Weiterreise', 1 );
INSERT INTO Timesheet ( BeginTime, EndTime, Description, TravelTypeId )
	VALUES ( '2014-01-03T17:00:00', '2014-01-03T19:00:00', 'Rückreise', 2 );

COMMIT TRANSACTION;";
				cmd.CommandType = CommandType.Text;
				cmd.ExecuteNonQuery();
			}
		}
	}
}


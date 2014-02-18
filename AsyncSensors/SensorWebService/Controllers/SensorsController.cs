using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;

namespace SensorWebService.Controllers
{
	public class SensorsController : ApiController
	{
		public async Task<IEnumerable<string>> Get()
		{
			// Note that the entire DB code is async using async/await
			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SensorDB"].ConnectionString))
			{
				await connection.OpenAsync();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Sensors'";
					if ((int)(await command.ExecuteScalarAsync()) == 0)
					{
						command.CommandText = "CREATE TABLE Sensors ( SensorName VARCHAR(50) PRIMARY KEY )";
						await command.ExecuteNonQueryAsync();
						command.CommandText = "INSERT INTO Sensors VALUES ( 'Temperature Sensor' )";
						await command.ExecuteNonQueryAsync();
						command.CommandText = "INSERT INTO Sensors VALUES ( 'Velocity Sensor' )";
						await command.ExecuteNonQueryAsync();
					}

					command.CommandText = "SELECT SensorName FROM Sensors";
					using (var reader = await command.ExecuteReaderAsync())
					{
						var result = new List<string>();
						while (await reader.ReadAsync())
						{
							result.Add(reader.GetString(0));
						}

						return result;
					}
				}
			}
		}
	}
}

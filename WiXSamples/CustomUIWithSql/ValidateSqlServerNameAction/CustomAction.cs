using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;

namespace ValidateSqlServerNameAction
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult ValidateSqlServerName(Session session)
		{
			var serverName = session["SQLSERVERNAME"];

			try
			{
				using (var conn = new SqlConnection(string.Format("Server={0};Database=master;Integrated Security=true;Connection Timeout=10", serverName)))
				{
					conn.Open();
				}

				session["SQLEXCEPTION"] = string.Empty;
			}
			catch (SqlException ex)
			{
				session["SQLEXCEPTION"] = ex.Message;
			}

			return ActionResult.Success;
		}
	}
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
	class Program
	{
		private const string connectionString = "Server=bwuxg4eqdg.database.windows.net;Database=AdventureWorks2012;User=dev;Password=P@zzw0rd!";
        private const string simpleQuery = @"SELECT Name, ProductNumber, ListPrice AS Price
			FROM Production.Product 
			ORDER BY Name ASC;";
		private const string complexQuery = @"SELECT DISTINCT pp.LastName, pp.FirstName 
			FROM Person.Person pp JOIN HumanResources.Employee e
				ON e.BusinessEntityID = pp.BusinessEntityID 
				WHERE pp.BusinessEntityID IN 
					(SELECT SalesPersonID FROM Sales.SalesOrderHeader
						WHERE SalesOrderID IN 
					(SELECT SalesOrderID FROM Sales.SalesOrderDetail
						WHERE ProductID IN 
					(SELECT ProductID FROM Production.Product p 
						WHERE ProductNumber = 'BK-M68B-42')));";
		private const string dataTableQuery = @"SELECT 'Total income is', ((OrderQty * UnitPrice) * (1.0 - UnitPriceDiscount)), ' for ',
			p.Name AS ProductName 
			FROM Production.Product AS p 
			INNER JOIN Sales.SalesOrderDetail AS sod
			ON p.ProductID = sod.ProductID 
			ORDER BY ProductName ASC;";

		static void Main(string[] args)
		{
			DoSimpleQuery();
			Debug.WriteLine("Done with simple query");

			DoSimpleQueryAsync().Wait();

			Debug.WriteLine("Starting complex query");
			DoComplexQuery();
			Debug.WriteLine("Complex query done!");

			var dt = FillDataTable(dataTableQuery);
		}

		static void DoSimpleQuery() => ExecuteQuery(simpleQuery);
		static async Task DoSimpleQueryAsync() => await ExecuteQueryAsync(simpleQuery);
		static void DoComplexQuery() => ExecuteQuery(complexQuery);

		static void ExecuteQuery(string query)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = $"-- Query start: {DateTime.Now}\n{query}";
                    using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read()) ;
					}
				}
			}
		}

		static async Task ExecuteQueryAsync(string query)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				await conn.OpenAsync();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = query;
					using (var reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync()) ;
					}
				}
			}
		}

		static DataTable FillDataTable(string query)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = query;
					using (var adapter = new SqlDataAdapter(cmd))
					{
						var dt = new DataTable();
						adapter.Fill(dt);
						return dt;
					}
				}
			}
		}
	}
}

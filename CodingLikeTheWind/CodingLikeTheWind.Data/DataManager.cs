using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;

namespace CodingLikeTheWind.Data
{
    public class DataManager
    {
        private string fileName = "SampleDatabase.sdf";
        private string connectionString = null;

        public DataManager()
        {
            CreateDatabase();
        }

        /// <summary>
        /// Deletes existing database files and creates a new database.
        /// </summary>
        private void CreateDatabase()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            connectionString = string.Format("Data Source={0}", fileName);

            var engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();

            CreateTables();
        }

        /// <summary>
        /// Creates the necessary database tables
        /// </summary>
        private void CreateTables()
        {
            using (var conn = new SqlCeConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE [Order] (OrderId int IDENTITY NOT NULL PRIMARY KEY, Description nvarchar(50), Amount int, UnitPrice float)";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts one order to the database
        /// </summary>
        /// <param name="description">Order description</param>
        /// <param name="amount">Amount</param>
        /// <param name="unitPrice">Price per unit</param>
        public void InsertOrder(string description, int amount, double unitPrice)
        {
            using (var conn = new SqlCeConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [Order] (Description, Amount, UnitPrice) VALUES (@description, @amount, @unitPrice)";
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@unitPrice", unitPrice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Returns the total sum of all orders
        /// </summary>
        /// <returns>total sum of all orders</returns>
        public double GetTotalAmount()
        {
            double result = 0;

            using (var conn = new SqlCeConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT SUM(Amount * UnitPrice) FROM [Order]";

                    object cmdResult = cmd.ExecuteScalar();
                    if (cmdResult != null && cmdResult is double)
                        result = (double)cmdResult;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads all orders from the database and returns them as a collection.
        /// </summary>
        /// <returns>Collection of order items</returns>
        public IList<Order> GetOrders()
        {
            IList<Order> lst = new List<Order>();

            using (var conn = new SqlCeConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT OrderId, Description, Amount, UnitPrice FROM [Order]";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lst.Add(new Order()
                            {
                                OrderId = reader.GetInt32(0),
                                Description = reader.GetString(1),
                                Amount = reader.GetInt32(2),
                                UnitPrice = reader.GetDouble(3)
                            });
                        }
                    }                    
                }
            }
            
            return lst;
        }
    }
}

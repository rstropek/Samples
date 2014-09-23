using System;

[assembly: CLSCompliant(true)]

namespace CodeAnalysisDemo.Corrected
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	public interface ISwiftItem<T>
	{
		string ItemName { get; }
		IEnumerable<T> Values { get; }
	}

	public interface INamedItem
	{
		string ItemName { get; }
	}

	public class SwiftItem<T> : ISwiftItem<T>
	{
		public string ItemName
		{
			get;
			set;
		}

		public IEnumerable<T> Values
		{
			get;
			set;
		}
	}

	public class Setting
	{
		public string SettingName { get; set; }
		public object SettingValue { get; set; }
	}

	public class SettingCollection : Collection<Setting>
	{
	}

	public class SwiftFile : IDisposable
	{
		private Stream underlying_File;
		private object header;
		private bool disposed;

		public SwiftFile(string fileName)
		{
			this.underlying_File = new FileStream(fileName, FileMode.Open);
			this.header = new object();
			this.Settings = new SettingCollection();
		}

		public SettingCollection Settings
		{
			get;
			private set;
		}

		public void AddObject<T>(SwiftItem<T> newObj)
		{
			if (newObj != null)
			{
				lock (this.header)
				{
					foreach (var item in newObj.Values)
					{
						var namedItem = item as INamedItem;
						if (namedItem != null)
						{
							// do something special with namedItem
						}
					}

					// do something with newObj
				}
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Will reference 'this' later")]
		public void CopyFile(string sourceFileName)
		{
			var stream = new FileStream(sourceFileName, FileMode.Open);
			StreamReader reader;
			try
			{
				reader = new StreamReader(stream);
			}
			catch
			{
				stream.Dispose();
				throw;
			}

			try
			{
				// do something with reader
			}
			finally
			{
				reader.Dispose();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Will reference 'this' later")]
		public void WriteToDatabase(SqlConnection conn, string tenant)
		{
			if (conn == null || (conn.State & ConnectionState.Open) == 0)
			{
				throw new ArgumentException("conn must not be null and must be open");
			}

			var cmd = conn.CreateCommand();
			cmd.CommandText = "INSERT INTO Target ( Tenant, Data ) VALUES ( @Tenant, @Data )";
			cmd.Parameters.Add("@Tenant", System.Data.SqlDbType.NVarChar, 100).Value = tenant;

			// Build rest of the command and execute it
		}

		public void Close()
		{
			try
			{
				this.underlying_File.Close();
			}
			catch
			{
				Console.WriteLine("Error");
				throw;
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.underlying_File.Dispose();
				}

				disposed = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SwiftFile()
		{
			Dispose(false);
		}
	}
}
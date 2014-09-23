namespace CodeAnalysisDemo
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.IO;

	public interface ISwiftItem<T>
	{
		string ItemName { get; }
		List<T> Values { get; }
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

		public List<T> Values
		{
			get;
			set;
		}
	}

	public class SwiftFile
	{
		private Stream underlying_File;
		private string fileName;
		private object header;

		public SwiftFile(string fileName)
		{
			this.underlying_File = new FileStream(fileName, FileMode.Open);
			this.fileName = fileName;
			this.header = new object();
		}

		public List<Tuple<string, object>> Settings
		{
			get;
			set;
		}

		public void AddObject<T>(SwiftItem<T> newObj)
		{
			lock (this.header)
			{
				var header = string.Format("{0}: {1}", newObj.ItemName, newObj.Values.Count);

				foreach (var item in newObj.Values)
				{
					if (item is INamedItem)
					{
						var namedItem = item as INamedItem;

						// do something special with namedItem
					}
				}

				// do something with newObj
			}
		}

		public void CopyFile(string source_file_name)
		{
			using (var reader = new StreamReader(new FileStream(source_file_name, FileMode.Open)))
			{
				// TODO: copy file here
			}
		}

		public void WriteToDatabase(SqlConnection conn, string tenant)
		{
			var cmd = conn.CreateCommand();
			cmd.CommandText = string.Format("INSERT INTO Target ( Tenant, Data ) VALUES ( {0}, @Data )", tenant);

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
			}
		}
	}
}
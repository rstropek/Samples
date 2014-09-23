//-------------------------------------------------------
// <copyright file="Bucket.cs" company="BASTA 2014">
//     Copyright (c) BASTA 2014 All rights reserved.
// </copyright>
//-------------------------------------------------------

namespace StyleCopDemo.Corrected
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Implements a bucket
	/// </summary>
	/// <typeparam name="T">Type of the elements in the bucket</typeparam>
	public class Bucket<T>
	{
		/// <summary>
		/// Internal helper to store data
		/// </summary>
		private Dictionary<string, T> data = new Dictionary<string, T>();

		/// <summary>
		/// Initializes a new instance of the Bucket class.
		/// </summary>
		public Bucket()
		{
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
			Justification = "No time to write documentation...")]
		public Bucket(IEnumerable<Tuple<string, T>> data)
		{
			foreach (var item in data)
			{
				this.data[item.Item1] = item.Item2;
			}
		}

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index">Index of the element to get.</param>
		/// <value>Element at the specified index.</value>
		/// <returns>Element at specified index.</returns>
		public T this[string index]
		{
			get
			{
				if (this.data.ContainsKey(index))
				{
					return this.data[index];
				}
				else
				{
					return default(T);
				}
			}
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <returns>Length of the dictionary</returns>
		public int GetLength()
		{
			return this.data.Keys.Count;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return this.Dump();
		}

		/// <summary>
		/// Dumps this instance.
		/// </summary>
		/// <returns>String representation of the dictionary</returns>
		private string Dump()
		{
			return this.data.Aggregate<KeyValuePair<string, T>, StringBuilder>(
			 new StringBuilder(),
			 (agg, item) =>
			 {
				 if (agg.Length > 0)
				 {
					 agg.Append(", ");
				 }

				 agg.AppendFormat("{0}: {1}", item.Key, item.Value.ToString());
				 return agg;
			 }).ToString();
		}
	}
}
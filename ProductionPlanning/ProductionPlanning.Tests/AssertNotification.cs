using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProductionPlanning.Tests
{
	/// <summary>
	/// Helper class to check property changed notifications
	/// </summary>
	/// <remarks>
	/// Note how we create a fluid API here
	/// </remarks>
	public static class AssertNotification
	{
		public interface IRaisedPropertyChanges
		{
			IRaisedPropertyChanges Raised(string propertyName);
			IRaisedPropertyChanges DidNotRaise(string propertyName);
        }

		private class RaisedPropertyChanges : IRaisedPropertyChanges
		{
			public HashSet<string> events = new HashSet<string>();

			public IRaisedPropertyChanges Raised(string propertyName)
			{
				if (!events.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"PropertChanged event was not raised for property {propertyName}");
				}

				return this;
			}

			public IRaisedPropertyChanges DidNotRaise(string propertyName)
			{
				if (events.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"PropertChanged event was raised for property {propertyName}");
				}

				return this;
			}
		}

		/// <summary>
		/// Adds an assertation extension method to object implementing INotifyPropertyChanged
		/// </summary>
		public static IRaisedPropertyChanges CheckIf<T>(this T obj, Action<T> body) 
			where T : INotifyPropertyChanged
		{
			var rpc = new RaisedPropertyChanges();

			PropertyChangedEventHandler handler = (_, ea) => rpc.events.Add(ea.PropertyName);
            obj.PropertyChanged += handler;
			body(obj);
			obj.PropertyChanged -= handler;

			return rpc;
		}
	}
}

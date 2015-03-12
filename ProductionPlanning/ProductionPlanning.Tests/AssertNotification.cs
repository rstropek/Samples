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

			IRaisedPropertyChanges RaisedErrorsChanged(string propertyName);
			IRaisedPropertyChanges DidNotRaiseErrorsChanged(string propertyName);
		}

		private class RaisedPropertyChanges : IRaisedPropertyChanges
		{
			// Note the use of readonly here
			public readonly HashSet<string> changeEvents = new HashSet<string>();
			public readonly HashSet<string> errorsChangedEvents = new HashSet<string>();

			#region Property changed handling
			public IRaisedPropertyChanges Raised(string propertyName)
			{
				if (!changeEvents.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"PropertChanged event was not raised for property {propertyName}");
				}

				return this;
			}

			public IRaisedPropertyChanges DidNotRaise(string propertyName)
			{
				if (changeEvents.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"PropertChanged event was raised for property {propertyName}");
				}

				return this;
			}
			#endregion

			#region Errors changed handling
			public IRaisedPropertyChanges RaisedErrorsChanged(string propertyName)
			{
				if (!errorsChangedEvents.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"ErrorsChanged event was not raised for property {propertyName}");
				}

				return this;
			}

			public IRaisedPropertyChanges DidNotRaiseErrorsChanged(string propertyName)
			{
				if (errorsChangedEvents.Contains(propertyName))
				{
					// Note string interpolation here
					Assert.Fail($"ErrorsChanged event was raised for property {propertyName}");
				}

				return this;
			}
			#endregion
		}

		/// <summary>
		/// Adds an assertation extension method to object implementing INotifyPropertyChanged
		/// </summary>
		public static IRaisedPropertyChanges CheckIf<T>(this T obj, Action<T> body) 
			where T : INotifyPropertyChanged
		{
			var rpc = new RaisedPropertyChanges();

			// Add handler for INotifyDataErrorInfo
			EventHandler<DataErrorsChangedEventArgs> errorsHandler = 
				(_, ea) => rpc.errorsChangedEvents.Add(ea.PropertyName);
			var notifyErrors = obj as INotifyDataErrorInfo;
			if (notifyErrors != null)
			{
				notifyErrors.ErrorsChanged += errorsHandler;
			}

			// Add handler for INotifyPropertyChanged
			PropertyChangedEventHandler handler = (_, ea) => rpc.changeEvents.Add(ea.PropertyName);
            obj.PropertyChanged += handler;

			body(obj);

			// Remove handlers
			obj.PropertyChanged -= handler;
			if (notifyErrors != null)
			{
				notifyErrors.ErrorsChanged -= errorsHandler;
			}

			return rpc;
		}
	}
}

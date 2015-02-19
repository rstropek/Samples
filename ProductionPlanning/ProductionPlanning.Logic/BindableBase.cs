using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProductionPlanning.Logic
{
	/// <summary>
	/// Acts as a helper base class for implementing INotifyPropertyChanged
	/// </summary>
	public abstract class BindableBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Sets the value of the backing field and raises PropertyChanged
		/// </summary>
		/// <typeparam name="T">Type of the backing field</typeparam>
		/// <param name="backingValue">Reference to the backing field to set</param>
		/// <param name="value">New value</param>
		/// <param name="propertyName">Name of the property using the backing field</param>
		/// <returns>
		/// True if the backing field's value was changed and PropertyChanged
		/// was raised. False if no changes were made (i.e. backingValue = value).
		/// </returns>
		protected bool SetProperty<T>(ref T backingValue, T value,
			// Note the use of CallerMemberName here
			[CallerMemberName]string propertyName = null)
		{
			// Note the use of Equals here instead of "backingValue == value"
			if (!object.Equals(backingValue, value))
			{
				backingValue = value;
				this.RaisePropertyChanged(propertyName);
				return true;
			}

			return false;
		}

		// Note null-conditional call inside expression-bodied function
		protected void RaisePropertyChanged([CallerMemberName]string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

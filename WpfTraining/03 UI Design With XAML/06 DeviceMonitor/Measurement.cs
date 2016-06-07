using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeviceMonitor
{
	public class Measurement : INotifyPropertyChanged
	{
		private DateTime TimestampValue;
		public DateTime Timestamp
		{
			get
			{
				return this.TimestampValue;
			}

			set
			{
				if (this.TimestampValue != value)
				{
					this.TimestampValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private double ValueValue;
		public double Value
		{
			get
			{
				return this.ValueValue;
			}

			set
			{
				if (this.ValueValue != value)
				{
					this.ValueValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private void RaisePropertyChanged([CallerMemberName]string propertyName = null) => 
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

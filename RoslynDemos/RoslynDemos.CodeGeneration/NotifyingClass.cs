using System.ComponentModel;

/*
	This sample demonstrates the use of Roslyn inside a compile-time T4 template.
	Note that you could generate the code using Roslyn, too. However, in this sample
	I wanted to show how to combine it with T4.
*/

namespace RoslynDemos
{
	public class TemperatureSensorSettingsViewModel 
		: INotifyPropertyChanged
	{ 
		private double 
			MeasureFrequencyValue;
		double 
			MeasureFrequency
		{
			get { return this.MeasureFrequencyValue; }
			set
			{
				if (this.MeasureFrequencyValue != value)
				{
					this.MeasureFrequencyValue = value;
					this.RaisePropertyChanged(
						"MeasureFrequency");
				}
			}
		}

		private double 
			AccuracyValue;
		double 
			Accuracy
		{
			get { return this.AccuracyValue; }
			set
			{
				if (this.AccuracyValue != value)
				{
					this.AccuracyValue = value;
					this.RaisePropertyChanged(
						"Accuracy");
				}
			}
		}

		internal TemperatureSensorSettings 
			TemperatureSensorSettings
		{
			get 
			{ 
				return new TemperatureSensorSettings(MeasureFrequency, Accuracy);
			}
		}

		public void RaisePropertyChanged(string propertyName) { /* […] */ }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}

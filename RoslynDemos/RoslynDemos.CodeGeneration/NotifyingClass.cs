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
			Accuracy2Value;
		double 
			Accuracy2
		{
			get { return this.Accuracy2Value; }
			set
			{
				if (this.Accuracy2Value != value)
				{
					this.Accuracy2Value = value;
					this.RaisePropertyChanged(
						"Accuracy2");
				}
			}
		}

		internal TemperatureSensorSettings 
			TemperatureSensorSettings
		{
			get 
			{ 
				return new TemperatureSensorSettings(MeasureFrequency, Accuracy2);
			}
		}

		public void RaisePropertyChanged(string propertyName) { /* […] */ }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}

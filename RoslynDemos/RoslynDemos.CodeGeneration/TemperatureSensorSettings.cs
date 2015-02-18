namespace RoslynDemos
{
	// Note that this class is immutable.
	public class TemperatureSensorSettings
	{
		public TemperatureSensorSettings(double measureFrequency, double accuracy)
		{
			this.MeasureFrequency = measureFrequency;
			this.Accuracy = accuracy;
		}

		public double MeasureFrequency { get; }
		public double Accuracy { get; }
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceMonitor
{
	public class Sensor
	{
		private Random rand = new Random();

		public Sensor()
		{
		}

		public Task<Measurement> MeasureAsync()
		{
			return Task.Run(() =>
				{
					// Simulate IO to get measurement
					Thread.Sleep(500);

					return new Measurement()
					{
						Timestamp = DateTime.Now,
						Value = this.rand.NextDouble()
					};
				});
		}
	}
}

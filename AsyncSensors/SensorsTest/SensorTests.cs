using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSensors;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorsTest
{
	[TestClass]
	public class SensorTests
	{
		[TestMethod]
		public async Task TestGetSensorsMocked()
		{
			var sensors = await Sensor.GetSensorsInternalAsync(
				(_, __) => Task.FromResult("['Temperature Sensor', 'Velocity Sensor']"));
			AssertGetSensorsResult(sensors);
		}

		[TestMethod]
		public async Task TestMeasure()
		{
			var sensor = new Sensor();
			await sensor.MeasureAsync();
		}

		private static void AssertGetSensorsResult(IEnumerable<string> sensors)
		{
			Assert.IsNotNull(sensors);
			Assert.AreEqual(2, sensors.Count());
			Assert.IsTrue(sensors.Contains("Temperature Sensor"));
			Assert.IsTrue(sensors.Contains("Velocity Sensor"));
		}
	}
}

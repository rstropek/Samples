using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSensors
{
	public class Sensor
	{
		public Sensor()
		{
		}

		internal static async Task<IEnumerable<string>> GetSensorsInternalAsync(
			Func<WebClient, string, Task<string>> contentLoader)
		{
			var uri = ConfigurationManager.AppSettings["ServiceUri"];
			using (var client = new WebClient())
			{
				var content = await contentLoader(client, uri);
				var sensors = JsonConvert.DeserializeObject<IEnumerable<string>>(content);
				return sensors;
			}
		}

		public static IEnumerable<string> GetSensors()
		{
			// Note that this implementation downloads the string synchronously
			return GetSensorsInternalAsync((client, uri) => Task.FromResult(client.DownloadString(uri))).Result;
		}

		public static Task<IEnumerable<string>> GetSensorsAsync()
		{
			// Note that this implementation downloads string asynchronously
			return GetSensorsInternalAsync(async (client, uri) => 
			{ 
				// Simulate long-running web request
				await Task.Delay(2000); 

				return await client.DownloadStringTaskAsync(uri); 
			});
		}

		public void Open(string sensorName)
		{
			// Simulate that opening connection takes a while...
			Thread.Sleep(1000);
			Debug.WriteLine("Sensor opened.");
		}

		public Task OpenAsync(string sensorName)
		{
			// Use device-specific API to establish connection.
			return Task.Run(() =>
			{
				this.Open(sensorName);
			});
		}

		public Task<double> MeasureAsync()
		{
			this.are.WaitOne();
			this.tcs = new TaskCompletionSource<double>();

			var timer = new System.Timers.Timer();
			timer.Interval = 250;
			timer.Elapsed += (_, __) =>
			{
				timer.Stop();
				this.tcs.SetResult(new Random().NextDouble());
				this.are.Set();
			};
			timer.Start();

			return tcs.Task;
		}

		private TaskCompletionSource<double> tcs;
		private AutoResetEvent are = new AutoResetEvent(true);

		public async Task<double> LongTermMeasureAsync(TimeSpan duration, CancellationToken token, IProgress<double> progress)
		{
			var sum = 0.0;
			var count = 0;

			var watch = new Stopwatch();
			watch.Start();
			do
			{
				sum += await this.MeasureAsync();
				count++;

				token.ThrowIfCancellationRequested();

				if (progress != null)
				{
					progress.Report(watch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds);
				}
			}
			while (watch.Elapsed <= duration);

			if (progress != null)
			{
				progress.Report(1.0);
			}

			return sum / count;
		}
	}
}

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsyncSensors
{
	public class MainWindowViewModel : NotificationObject
	{
		private Sensor sensor = new Sensor();

		private CancellationTokenSource cts;

		public MainWindowViewModel()
		{
			this.OpenSensorCommandValue = new DelegateCommand(
				() => this.OpenAsync(),
				() => !string.IsNullOrWhiteSpace(this.SelectedSensor)
					&& !this.IsOpeningSensor);

			this.LongTermMeasurementCommandValue = new DelegateCommand(
				async () => await OnLongTermMeasurement(),
				() => this.IsOpen && !this.IsLongTermMeasurementRunning);

			this.CancelLongTermMeasurementCommandValue = new DelegateCommand(
				async () =>
				{
					if (this.cts != null)
					{
						if (await this.ConfirmationHandler())
						{
							this.cts.Cancel();
						}
					}
				},
				() => this.IsLongTermMeasurementRunning);

			this.RefreshSensorListAsync();
		}

		public Func<Task<bool>> ConfirmationHandler { get; set; }

		private async Task OnLongTermMeasurement()
		{
			this.cts = new CancellationTokenSource();
			this.IsLongTermMeasurementRunning = true;
			try
			{
				this.Measurement = await this.sensor.LongTermMeasureAsync(
					TimeSpan.FromSeconds(3),
					this.cts.Token,
					new Progress<double>(p => this.Progress = p));
			}
			catch (OperationCanceledException)
			{
				Debug.WriteLine("Cancelled...");
			}

			this.IsLongTermMeasurementRunning = false;
		}

		public void Init()
		{
			var timer = new System.Timers.Timer();
			timer.Interval = 100;
			timer.Elapsed += async (_, __) =>
			{
				timer.Stop();
				App.Current.Dispatcher.BeginInvoke(new Action(async () =>
				{
					this.Measurement = await this.sensor.MeasureAsync();
					timer.Start();
				}));
			};
			timer.Start();
		}

		private double MeasurementValue;
		public double Measurement
		{
			get
			{
				return this.MeasurementValue;
			}

			set
			{
				if (this.MeasurementValue != value)
				{
					this.MeasurementValue = value;
					this.RaisePropertyChanged(() => this.Measurement);
				}
			}
		}

		private double ProgressValue;
		public double Progress
		{
			get
			{
				return this.ProgressValue;
			}

			set
			{
				if (this.ProgressValue != value)
				{
					this.ProgressValue = value;
					this.RaisePropertyChanged(() => this.Progress);
				}
			}
		}

		private async Task OpenAsync()
		{
			if (!string.IsNullOrWhiteSpace(this.SelectedSensor))
			{
				this.IsOpeningSensor = true;
				try
				{
					await this.sensor.OpenAsync(this.SelectedSensor);
					this.IsOpen = true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					throw;
				}
				finally
				{
					this.IsOpeningSensor = false;
				}
			}
			else
			{
				Debug.WriteLine("Cannot open sensor; no sensor selected.");
			}
		}

		private bool IsOpeningSensorValue;
		public bool IsOpeningSensor
		{
			get
			{
				return this.IsOpeningSensorValue;
			}

			set
			{
				if (this.IsOpeningSensorValue != value)
				{
					this.IsOpeningSensorValue = value;
					this.RaisePropertyChanged(() => this.IsOpeningSensor);
					this.OpenSensorCommandValue.RaiseCanExecuteChanged();
				}
			}
		}

		private bool IsOpenValue;
		public bool IsOpen
		{
			get
			{
				return this.IsOpenValue;
			}

			set
			{
				if (this.IsOpenValue != value)
				{
					this.IsOpenValue = value;
					this.RaisePropertyChanged(() => this.IsOpen);
					this.LongTermMeasurementCommandValue.RaiseCanExecuteChanged();
				}
			}
		}

		private bool IsLongTermMeasurementRunningValue;
		public bool IsLongTermMeasurementRunning
		{
			get
			{
				return this.IsLongTermMeasurementRunningValue;
			}

			set
			{
				if (this.IsLongTermMeasurementRunningValue != value)
				{
					this.IsLongTermMeasurementRunningValue = value;
					this.RaisePropertyChanged(() => this.IsLongTermMeasurementRunning);
					this.LongTermMeasurementCommandValue.RaiseCanExecuteChanged();
					this.CancelLongTermMeasurementCommandValue.RaiseCanExecuteChanged();
				}
			}
		}

		public async Task RefreshSensorListAsync()
		{
			this.IsRefreshingSensorList = true;
			this.AvailableSensors = await Sensor.GetSensorsAsync();
			this.IsRefreshingSensorList = false;
		}

		private bool IsRefreshingSensorListValue;
		public bool IsRefreshingSensorList
		{
			get
			{
				return this.IsRefreshingSensorListValue;
			}

			set
			{
				if (this.IsRefreshingSensorListValue != value)
				{
					this.IsRefreshingSensorListValue = value;
					this.RaisePropertyChanged(() => this.IsRefreshingSensorList);
				}
			}
		}

		private string SelectedSensorValue;
		public string SelectedSensor
		{
			get
			{
				return this.SelectedSensorValue;
			}

			set
			{
				if (this.SelectedSensorValue != value)
				{
					this.SelectedSensorValue = value;
					this.RaisePropertyChanged(() => this.SelectedSensor);
					this.OpenSensorCommandValue.RaiseCanExecuteChanged();
				}
			}
		}

		private DelegateCommand OpenSensorCommandValue;
		public ICommand OpenSensorCommand
		{
			get { return this.OpenSensorCommandValue; }
		}

		private DelegateCommand LongTermMeasurementCommandValue;
		public ICommand LongTermMeasurementCommand
		{
			get { return this.LongTermMeasurementCommandValue; }
		}

		private DelegateCommand CancelLongTermMeasurementCommandValue;
		public ICommand CancelLongTermMeasurementCommand
		{
			get { return this.CancelLongTermMeasurementCommandValue; }
		}

		private IEnumerable<string> AvailableSensorsValue;
		public IEnumerable<string> AvailableSensors
		{
			get
			{
				return this.AvailableSensorsValue;
			}

			set
			{
				if (this.AvailableSensorsValue != value)
				{
					this.AvailableSensorsValue = value;
					this.RaisePropertyChanged(() => this.AvailableSensors);
				}
			}
		}
	}
}

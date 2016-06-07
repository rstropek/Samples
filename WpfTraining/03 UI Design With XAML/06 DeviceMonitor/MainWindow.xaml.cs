using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace DeviceMonitor
{
    public partial class MainWindow : Window
	{
		private Sensor sensor = new Sensor();
		private DispatcherTimer timer;
		private ObservableCollection<Measurement> measurements;

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this.measurements = new ObservableCollection<Measurement>();

			this.timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += async (_, __) => this.measurements.Insert(0, await sensor.MeasureAsync()); 
			timer.Start();
		}
	}
}

using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;

namespace Samples
{
	public class FreeSpaceInfo : DependencyObject
	{
		private DispatcherTimer monitorFreeSpaceTimer;
		private DriveInfo currentDriveInfo = null;

		public FreeSpaceInfo()
		{
			// start a timer that monitors the free disk space every second
			monitorFreeSpaceTimer = new DispatcherTimer();
			monitorFreeSpaceTimer.Tick += new EventHandler(freeSpaceMonitorTimer_Tick);
			monitorFreeSpaceTimer.Interval = new TimeSpan(0, 0, 1);
			monitorFreeSpaceTimer.Start();
		}

		private void freeSpaceMonitorTimer_Tick(object sender, EventArgs e)
		{
			// stop timer while processing
			monitorFreeSpaceTimer.Stop();

			// has the drive-property been set?
			if (currentDriveInfo != null)
			{
				// calculate the free space ratio
				double newRatio = Convert.ToDouble(currentDriveInfo.TotalFreeSpace) / currentDriveInfo.TotalSize;
				// check if free space ratio has changed
				if (newRatio != FreeSpaceRatio)
					// set dependency property
					SetValue(FreeSpaceRatioProperty, newRatio);
			}

			// start timer after processing
			monitorFreeSpaceTimer.Start();
		}

		#region "Drive" dependency property
		public static readonly DependencyProperty DriveProperty =
			DependencyProperty.Register("Drive", typeof(string), typeof(FreeSpaceInfo),
			new PropertyMetadata( new PropertyChangedCallback(OnDriveChanged)));
		public string Drive
		{
			get { return (string)GetValue(DriveProperty); }
			set { SetValue(DriveProperty, value); }
		}
		public static void OnDriveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FreeSpaceInfo o = (FreeSpaceInfo)d;
			// check if the drive property is empty
			if (((string)e.NewValue).Length > 0)
			{
				// get data about the drive
				o.currentDriveInfo = new DriveInfo((string)e.NewValue);
				// set dependency property
				d.SetValue(FreeSpaceRatioProperty, 
					Convert.ToDouble(o.currentDriveInfo.TotalFreeSpace) / o.currentDriveInfo.TotalSize);
			}
			else
				// no drive has been selected -> set free space ratio to zero
				d.SetValue(FreeSpaceRatioProperty, 0.0);
		}
		#endregion

		#region "FreeSpaceRatio" dependency property
		public double FreeSpaceRatio
		{
			// this property is read only -> no set is implemented
			get { return (double)GetValue(FreeSpaceRatioProperty); }
		}
		public static readonly DependencyProperty FreeSpaceRatioProperty =
			DependencyProperty.Register("FreeSpaceRatio", typeof(double), typeof(FreeSpaceInfo));
		#endregion
	}
}

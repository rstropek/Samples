using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;

namespace Samples
{
	public class FreeSpaceInfo : DependencyObject
	{
		private DriveInfo currentDriveInfo = null;

		public FreeSpaceInfo()
		{
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
				// no drive has been selected -> set free space ratio to 0
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

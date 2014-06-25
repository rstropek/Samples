using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Samples
{
	public partial class FreeSpaceWatcherWindow : System.Windows.Window
	{
		public FreeSpaceWatcherWindow()
		{
			InitializeComponent();
			//DrivesCombo.SelectionChanged += new SelectionChangedEventHandler(DrivesCombo_SelectionChanged);
		}

		//private void DrivesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//    ((FreeSpaceInfo)FindResource("spaceInfo")).Drive = e.AddedItems[0].ToString();
		//}

		public static DriveInfo[] Drives
		{
			get { return DriveInfo.GetDrives(); }
		}
	}
}
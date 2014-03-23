namespace Snek.ToolsSample.WpfClient
{
	using System.Windows;
	using System.Windows.Controls;

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			// Note that this program has a memory leak. It is used to demonstrate how to use
			// a memory profiler to find and correct the leak.
			this.InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// If tab item with travel list is selected...
			if (TravelsItem.IsSelected)
			{
				// ...set the content of the tab to a user control.
				TravelsItem.Content = new Travels();
			}
		}
	}
}

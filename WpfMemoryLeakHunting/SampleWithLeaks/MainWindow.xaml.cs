// Note that this example CONTAINS MEMORY LEAKS. I use it in workshops to show how you can use
// memory profilers to find typical .NET memory leaks.

using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication19
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// Export print menu via MEF so that all views can subscribe to click event.
			((App)Application.Current).Container.ComposeExportedValue<MenuItem>("PrintMenuItem", this.PrintMenuItem);
		}

		private void OnCreateNewTab(object sender, RoutedEventArgs e)
		{
			this.Content.Items.Add(
				new TabItem()
				{
					Header = "Customers",
					// Note that we use MEF to create instance here.
					Content = ((App)Application.Current).Container.GetExportedValue<UserControl>("CustomerView")
				});
		}

		private void OnCloseTab(object sender, RoutedEventArgs e)
		{
			var selectedView = this.Content.SelectedItem;
			if (selectedView != null)
			{
				// Remove selected tab
				this.Content.Items.Remove(selectedView);
			}
		}
	}
}

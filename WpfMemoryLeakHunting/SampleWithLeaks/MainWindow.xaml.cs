// Note that this example CONTAINS MEMORY LEAKS. I use it in workshops to show how you can use
// memory profilers to find typical .NET memory leaks.

using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication19
{
	public partial class MainWindow : Window
	{
		private App currentApp;

		public MainWindow()
		{
			InitializeComponent();

			this.currentApp = ((App)Application.Current);
			// Export print menu via MEF so that all views can subscribe to click event.
			((App)Application.Current).Container.ComposeExportedValue<MenuItem>("PrintMenuItem", this.PrintMenuItem);
		}

		private void OnCreateNewTab(object sender, RoutedEventArgs e)
		{
			// Note that we use MEF to create instance here.
			var view = this.currentApp.Container.GetExportedValue<UserControl>("CustomerView");

			this.Content.Items.Add(
				new TabItem()
				{
					Header = "Customers",
					Content = view
				});
		}

		private void OnCloseTab(object sender, RoutedEventArgs e)
		{
			var selectedView = this.Content.SelectedItem as TabItem;
			if (selectedView != null)
			{
				// Remove selected tab
				this.Content.Items.Remove(selectedView);
			}
		}
	}
}

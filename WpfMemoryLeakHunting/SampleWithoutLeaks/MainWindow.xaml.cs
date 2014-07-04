using System;
using System.Collections.Generic;
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
			currentApp.Container.ComposeExportedValue<MenuItem>("PrintMenuItem", this.PrintMenuItem);
		}

		// Dictionary to remember exports that led to view objects
		private Dictionary<UserControl, Lazy<UserControl>> exports = new Dictionary<UserControl, Lazy<UserControl>>();

		private void OnCreateNewTab(object sender, RoutedEventArgs e)
		{
			// Get the export that can be used to generate a new instance
			var viewExport = currentApp.Container.GetExport<UserControl>("CustomerView");
			// Store the export and the generated instance
			this.exports.Add(viewExport.Value, viewExport);

			this.Content.Items.Add(
				new TabItem()
				{
					Header = "Customers",
					Content = viewExport.Value
				});
		}

		private void OnCloseTab(object sender, RoutedEventArgs e)
		{
			var selectedTabItem = this.Content.SelectedItem as TabItem;
			if (selectedTabItem != null)
			{
				var selectedView = selectedTabItem.Content as UserControl;

				// Remove selected tab
				this.Content.Items.Remove(selectedTabItem);
				// Release the export; releases the created instance, too
				currentApp.Container.ReleaseExport(this.exports[selectedView]);
				this.exports.Remove(selectedView);
			}
		}
	}
}

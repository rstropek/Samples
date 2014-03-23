namespace Snek.ToolsSample.WpfClient
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	using Snek.ToolsSample.Logic;

	public partial class Travels : UserControl
	{
		private readonly TimesheetDatabaseContext context = new TimesheetDatabaseContext();

		public Travels()
		{
			this.InitializeComponent();
			this.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// Load data into the list
			this.RefreshList();

			// Subscribe to timer to refresh data automatically
			var app = (App)Application.Current;
			app.AutoRefreshTimer.Elapsed += (o, args) => app.Dispatcher.BeginInvoke(new Action(this.RefreshList));
		}

		private void RefreshList()
		{
			this.DataContext = new { TravelList = this.context.FindAndProcessTravels(), LastRefreshDateTime = DateTime.Now };
		}
	}
}

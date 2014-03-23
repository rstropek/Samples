namespace Snek.ToolsSample.WpfClient
{
	using System.Timers;
	using System.Windows;

	public partial class App : Application
	{
		public readonly Timer AutoRefreshTimer = new Timer();

		protected override void OnStartup(StartupEventArgs e)
		{
			// Setup a timer that can be used to auto-refresh data.
			this.AutoRefreshTimer.Interval = 5000;
			this.AutoRefreshTimer.Start();

			base.OnStartup(e);
		}
	}
} 

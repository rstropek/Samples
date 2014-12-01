using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace ServiceToInstall
{
	/// <summary>
	/// Represents a very simple service that writes entries to Windows Event Log
	/// </summary>
	public partial class EventLoggingService : ServiceBase
	{
		private Timer timer;

		public EventLoggingService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			if (!EventLog.SourceExists("EventLoggingService"))
			{
				EventLog.CreateEventSource("EventLoggingService", "Application");
			}
			else
			{
				EventLog.WriteEntry("EventLoggingService", "Starting ...", EventLogEntryType.Information);
			}

			this.timer = new Timer(30000);
			this.timer.Elapsed += (_, __) => EventLog.WriteEntry(
				"EventLoggingService",
				string.Format("Tick {0} ...", DateTime.Now.Ticks), 
				EventLogEntryType.Information);
			this.timer.Start();
		}

		protected override void OnStop()
		{
			EventLog.WriteEntry("EventLoggingService", "Stoping ...", EventLogEntryType.Information);

			this.timer.Stop();
			this.timer.Dispose();
			this.timer = null;
		}
	}
}

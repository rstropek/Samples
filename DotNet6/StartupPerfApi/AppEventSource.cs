using System.Diagnostics.Tracing;

namespace StartupPerfApi
{
	[EventSource(Name = "StartupPerf")]
	public sealed class AppEventSource : EventSource
	{
		public static readonly AppEventSource Log = new();

		[Event(1)]
		public void ConfigureStarting() { WriteEvent(1, ""); }

		[Event(2)]
		public void ConfigureFinished() { WriteEvent(2, ""); }
	}
}

using System.Windows;
using System.Threading;

namespace FreeSpaceWatcher
{
	public partial class App : Application
	{
		private DispatcherQueue dispatcherQueueForm;
		private Thread queueThread;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// start a new thread for the window monitoring the dispatcher
			queueThread = new Thread(new ParameterizedThreadStart(QueueThreadProc));
			queueThread.SetApartmentState(ApartmentState.STA);
			queueThread.IsBackground = true;
			// pass the dispatcher of the main window as a parameter to the new thread
			queueThread.Start(Dispatcher);
		}

		private void QueueThreadProc( object dispatcher )
		{
			// start window in new thread
			dispatcherQueueForm = new DispatcherQueue((System.Windows.Threading.Dispatcher)dispatcher);
			dispatcherQueueForm.Show();
			// a dispatcher is automatically created by WPF
			System.Windows.Threading.Dispatcher.Run();
		}
	}
}
using System;
using System.Windows;
using System.Windows.Threading;

namespace FreeSpaceWatcher
{
    public partial class DispatcherQueue : Window
	{
		private delegate void QueueEventsDelegate();
		private DispatcherTimer perSecondTimer;
		private int lastCounterValue = 0;
		private System.Windows.Threading.Dispatcher dispatcher;

		public DispatcherQueue(System.Windows.Threading.Dispatcher dispatcher)
		{
			InitializeComponent();

			// set data context to enable binding
			DataContext = this;

			// start a timer that is used to update the input field every second
			perSecondTimer = new DispatcherTimer(DispatcherPriority.Normal);
			perSecondTimer.Tick += new EventHandler(perSecondTimer_Tick);
			perSecondTimer.Interval = new TimeSpan(0, 0, 1);
			perSecondTimer.Start();

			// remember the dispatcher that should be monitored
			this.dispatcher = dispatcher;

			// add a hook to the dispatcher that is monitored
			dispatcher.Hooks.OperationPosted += new System.Windows.Threading.DispatcherHookEventHandler(Hooks_OperationPosted);
		}

		void perSecondTimer_Tick(object sender, EventArgs e)
		{
			// set the dependency property
			SetValue(OperationsPerSecondCounterProperty, TotalOperationsCounter - lastCounterValue);

			// remember the last value for next second
			lastCounterValue = TotalOperationsCounter;
		}

		void Hooks_OperationPosted(object sender, System.Windows.Threading.DispatcherHookEventArgs e)
		{
			// this event handler runs in a separate thread; therefore we have to use
			// the window's dispatcher queue to schedule a procedure incrementing
			// the counter (=a dependency property).
			Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
				new QueueEventsDelegate(IncrementOperationsCounter));
		}

		private void IncrementOperationsCounter()
		{
			// increment the dependency property
			SetValue(TotalOperationsCounterProperty, TotalOperationsCounter + 1);
		}

		#region "TotalOperationsCounter" dependency property
		public int TotalOperationsCounter
		{
			get { return (int)GetValue(TotalOperationsCounterProperty); }
		}

		public static readonly DependencyProperty TotalOperationsCounterProperty =
			DependencyProperty.Register("TotalOperationsCounter", typeof(int), typeof(DispatcherQueue),
			new PropertyMetadata(0));
		#endregion

		#region "OperationsPerSecondCounter" dependency property
		public int OperationsPerSecondCounter
		{
			get { return (int)GetValue(OperationsPerSecondCounterProperty); }
		}

		public static readonly DependencyProperty OperationsPerSecondCounterProperty =
			DependencyProperty.Register("OperationsPerSecondCounter", typeof(int), typeof(DispatcherQueue),
			new PropertyMetadata(0));
		#endregion
	}
}
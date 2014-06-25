using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ControlLibrary
{
	[TemplateVisualState(Name = "FirstHalf", GroupName = "HalfMinute")]
	[TemplateVisualState(Name = "SecondHalf", GroupName = "HalfMinute")]
	public class Clock : Control
	{
		private DispatcherTimer timer;

		static Clock()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			this.UpdateDateTime();

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000 - DateTime.Now.Millisecond);
			timer.Tick += new EventHandler(Timer_Tick);
			timer.Start();
		}

		public override void OnApplyTemplate()
		{
			this.UpdateStates(false);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			this.UpdateDateTime();
		}

		private void UpdateDateTime()
		{
			this.DateTime = System.DateTime.Now;
		}

		#region DateTime property
		public DateTime DateTime
		{
			get { return (DateTime)GetValue(DateTimeProperty); }
			private set { SetValue(DateTimeProperty, value); }
		}

		public static DependencyProperty DateTimeProperty = DependencyProperty.Register(
				"DateTime", typeof(DateTime), typeof(Clock),
				new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDateTimeInvalidated)));

		public static readonly RoutedEvent DateTimeChangedEvent =
			EventManager.RegisterRoutedEvent("DateTimeChanged", RoutingStrategy.Bubble,
			typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(Clock));

		protected virtual void OnDateTimeChanged(DateTime oldValue, DateTime newValue)
		{
			RoutedPropertyChangedEventArgs<DateTime> args = new RoutedPropertyChangedEventArgs<DateTime>(oldValue, newValue);
			args.RoutedEvent = Clock.DateTimeChangedEvent;
			this.RaiseEvent(args);
		}

		private static void OnDateTimeInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Clock clock = (Clock)d;

			var oldValue = (DateTime)e.OldValue;
			var newValue = (DateTime)e.NewValue;

			clock.OnDateTimeChanged(oldValue, newValue);
			clock.UpdateStates(true);
		}
		#endregion

		#region VisualStates
		private void UpdateStates(bool useTransitions)
		{
			if (this.DateTime.Second >= 30)
			{
				VisualStateManager.GoToState(this, "SecondHalf", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "FirstHalf", useTransitions);
			}
		}
		#endregion
	}
}

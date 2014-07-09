using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BookshelfConfigurator.Controls
{
	[TemplatePart(Name = "UpButtonElement", Type = typeof(RepeatButton))]
	[TemplatePart(Name = "DownButtonElement", Type = typeof(RepeatButton))]
	[TemplateVisualState(Name = "Valid", GroupName = "ValueStates")]
	[TemplateVisualState(Name = "Invalid", GroupName = "ValueStates")]
	public class NumericUpDown : Control
	{
		public NumericUpDown()
		{
			DefaultStyleKey = typeof(NumericUpDown);
			this.IsTabStop = true;
		}

		#region Value property
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new PropertyMetadata(new PropertyChangedCallback(ValueChangedCallback)));

		public int Value
		{
			get	{ return (int)GetValue(ValueProperty);	}
			set	{ SetValue(ValueProperty, value); }
		}

		private static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			NumericUpDown ctl = (NumericUpDown)obj;
			int newValue = (int)args.NewValue;

			// Call UpdateStates because the Value might have caused the
			// control to change ValueStates.
			ctl.UpdateStates(true);

			// Call OnValueChanged to raise the ValueChanged event.
			ctl.OnValueChanged(new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent, newValue));
		}

		public static readonly RoutedEvent ValueChangedEvent =
			EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(ValueChangedEventHandler), typeof(NumericUpDown));

		public event ValueChangedEventHandler ValueChanged
		{
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}

		protected virtual void OnValueChanged(ValueChangedEventArgs e)
		{
			// Raise the ValueChanged event so applications can be alerted
			// when Value changes.
			RaiseEvent(e);
		}
		#endregion

		#region Maximum property
		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new PropertyMetadata(new PropertyChangedCallback(MaximumChangedCallback)));

		public int Maximum
		{
			get { return (int)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		private static void MaximumChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			NumericUpDown ctl = (NumericUpDown)obj;
			int newValue = (int)args.NewValue;

			// Call UpdateStates because the Maximum might have caused the
			// control to change ValueStates.
			ctl.UpdateStates(true);

			// Call OnMaximumChanged to raise the ValueChanged event.
			ctl.OnMaximumChanged(new ValueChangedEventArgs(NumericUpDown.MaximumChangedEvent, newValue));
		}

		public static readonly RoutedEvent MaximumChangedEvent =
			EventManager.RegisterRoutedEvent("MaximumChanged", RoutingStrategy.Direct, typeof(ValueChangedEventHandler), typeof(NumericUpDown));

		public event ValueChangedEventHandler MaximumChanged
		{
			add { AddHandler(MaximumChangedEvent, value); }
			remove { RemoveHandler(MaximumChangedEvent, value); }
		}

		protected virtual void OnMaximumChanged(ValueChangedEventArgs e)
		{
			// Raise the MaximumChanged event so applications can be alerted
			// when Maximum changes.
			RaiseEvent(e);
		}
		#endregion

		private void UpdateStates(bool useTransitions)
		{
			if (Value >= 0 && Value <= Maximum)
			{
				VisualStateManager.GoToState(this, "Valid", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Invalid", useTransitions);
			}
		}

		public override void OnApplyTemplate()
		{
			UpButtonElement = GetTemplateChild("UpButton") as RepeatButton;
			DownButtonElement = GetTemplateChild("DownButton") as RepeatButton;

			UpdateStates(false);
		}

		#region Down button
		private RepeatButton downButtonElement;

		private RepeatButton DownButtonElement
		{
			get { return downButtonElement; }
			set
			{
				if (downButtonElement != null)
				{
					downButtonElement.Click -= new RoutedEventHandler(downButtonElement_Click);
				}
				downButtonElement = value;

				if (downButtonElement != null)
				{
					downButtonElement.Click += new RoutedEventHandler(downButtonElement_Click);
				}
			}
		}

		void downButtonElement_Click(object sender, RoutedEventArgs e)
		{
			Value--;
		}
		#endregion

		#region Up button
		private RepeatButton upButtonElement;

		private RepeatButton UpButtonElement
		{
			get	{ return upButtonElement; }
			set
			{
				if (upButtonElement != null)
				{
					upButtonElement.Click -= new RoutedEventHandler(upButtonElement_Click);
				}

				upButtonElement = value;
				if (upButtonElement != null)
				{
					upButtonElement.Click += new RoutedEventHandler(upButtonElement_Click);
				}
			}
		}

		void upButtonElement_Click(object sender, RoutedEventArgs e)
		{
			Value++;
		}
		#endregion 
	}

	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

	public class ValueChangedEventArgs : RoutedEventArgs
	{
		private int _value;

		public ValueChangedEventArgs(RoutedEvent id, int num)
		{
			_value = num;
			RoutedEvent = id;
		}

		public int Value
		{
			get { return _value; }
		}
	}
}

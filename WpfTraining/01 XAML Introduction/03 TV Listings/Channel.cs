using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Samples
{
	[ContentProperty("Broadcasts")]
	public class Channel : DependencyObject
	{
		public Channel()
		{
			Broadcasts = new ObservableCollection<Broadcast>();
		}

		// Dependency Property Name
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(TVListings), new UIPropertyMetadata(string.Empty));

		// Dependency Property Broadcasts
		public ObservableCollection<Broadcast> Broadcasts
		{
			get { return (ObservableCollection<Broadcast>)GetValue(BroadcastsProperty); }
			set { SetValue(BroadcastsProperty, value); }
		}

		public static readonly DependencyProperty BroadcastsProperty =
			DependencyProperty.Register("Broadcasts", typeof(ObservableCollection<Broadcast>), typeof(Channel), new UIPropertyMetadata());
	}
}

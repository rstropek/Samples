using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Samples
{
	[ContentProperty("Channels")]
	public class TVListings : DependencyObject
	{
		public TVListings()
		{
			Channels = new ObservableCollection<Channel>();
		}

		// Dependency Property Channels
		public ObservableCollection<Channel> Channels
		{
			get { return (ObservableCollection<Channel>)GetValue(ChannelsProperty); }
			set { SetValue(ChannelsProperty, value); }
		}

		public static readonly DependencyProperty ChannelsProperty =
			DependencyProperty.Register("Channels", typeof(ObservableCollection<Channel>), typeof(TVListings), new UIPropertyMetadata());

		// Attached Property StartTime
		public static DateTime GetStartTime(DependencyObject obj)
		{
			return (DateTime)obj.GetValue(StartTimeProperty);
		}

		public static void SetStartTime(DependencyObject obj, DateTime value)
		{
			obj.SetValue(StartTimeProperty, value);
		}

		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.RegisterAttached("StartTime", typeof(DateTime), typeof(TVListings), new UIPropertyMetadata(new DateTime(1, 1, 1, 0, 0, 0)));
	}
}

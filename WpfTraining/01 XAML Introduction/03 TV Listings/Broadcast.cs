using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Samples
{
	public class Broadcast : DependencyObject
	{
		public Broadcast()
		{
			Actors = new ObservableCollection<Person>();
			Directors = new ObservableCollection<Person>();
		}

		// Dependency Property Name
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(Broadcast), new UIPropertyMetadata(string.Empty));

		// Dependency Property Actors
		[TypeConverter(typeof(PersonTypeConverter))]
		public ObservableCollection<Person> Actors
		{
			get { return (ObservableCollection<Person>)GetValue(ActorsProperty); }
			set { SetValue(ActorsProperty, value); }
		}

		public static readonly DependencyProperty ActorsProperty =
			DependencyProperty.Register("Actors", typeof(ObservableCollection<Person>), typeof(Broadcast), new UIPropertyMetadata());

		// Dependency Property Directors
		[TypeConverter(typeof(PersonTypeConverter))]
		public ObservableCollection<Person> Directors
		{
			get { return (ObservableCollection<Person>)GetValue(DirectorsProperty); }
			set { SetValue(DirectorsProperty, value); }
		}

		public static readonly DependencyProperty DirectorsProperty =
			DependencyProperty.Register("Directors", typeof(ObservableCollection<Person>), typeof(Broadcast), new UIPropertyMetadata());
	}
}

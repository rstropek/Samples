using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Samples
{
	public class Person : DependencyObject
	{
		// Dependency Property FirstName
		public string FirstName
		{
			get { return (string)GetValue(FirstNameProperty); }
			set { SetValue(FirstNameProperty, value); }
		}

		public static readonly DependencyProperty FirstNameProperty =
			DependencyProperty.Register("FirstName", typeof(string), typeof(Person), new UIPropertyMetadata(string.Empty));

		// Dependency Property LastName
		public string LastName
		{
			get { return (string)GetValue(LastNameProperty); }
			set { SetValue(LastNameProperty, value); }
		}

		public static readonly DependencyProperty LastNameProperty =
			DependencyProperty.Register("LastName", typeof(string), typeof(Person), new UIPropertyMetadata(string.Empty));
	}
}

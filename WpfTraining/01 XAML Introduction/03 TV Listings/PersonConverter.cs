using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;

namespace Samples
{
	[ValueConversion(typeof(ObservableCollection<Person>), typeof(String))]
	public class PersonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string personList = "";
			ObservableCollection<Person> persons = (ObservableCollection<Person>)value;
			for (int i = 0; i < persons.Count; i++)
			{
				if (personList.Length > 0)
				{
					personList += ", ";
				}
				personList += persons[i].FirstName + " " + persons[i].LastName;
			}
			return personList;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Samples
{
    [ValueConversion(typeof(ObservableCollection<Person>), typeof(String))]
	public class PersonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var personList = "";
			var persons = (ObservableCollection<Person>)value;
			for (var i = 0; i < persons.Count; i++)
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

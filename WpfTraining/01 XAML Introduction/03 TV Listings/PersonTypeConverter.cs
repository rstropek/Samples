using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Samples
{
	public class PersonTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			ObservableCollection<Person> persons = new ObservableCollection<Person>();
			string personList = (string)value;
			string[] personArray = personList.Split(',');
			for (int i = 0; i < personArray.Length; i++)
			{
				Person p = new Person();
				string[] person = personArray[i].Trim().Split(' ');
				p.FirstName = person[0];
				p.LastName = person[1];
				persons.Add(p);
			}
			return persons;
		}
	}
}

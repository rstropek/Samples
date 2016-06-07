using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            var persons = new ObservableCollection<Person>();
            var personList = (string)value;
            var personArray = personList.Split(',');
            for (var i = 0; i < personArray.Length; i++)
            {
                var person = personArray[i].Trim().Split(' ');
                persons.Add(new Person()
                {
                    FirstName = person[0],
                    LastName = person[1]
                });
            }

            return persons;
        }
    }
}

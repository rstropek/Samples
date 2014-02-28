using Roslyn.Scripting.CSharp;
using System;
using System.Windows.Data;

namespace RoslynScripting
{
	public class ScriptConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var engine = new ScriptEngine();
			var session = engine.CreateSession();

			// Note that we use session.Execute<T> to get a delegate
			Func<string, object> f = session.Execute<Func<string, object>>(parameter.ToString());
			return f(value.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

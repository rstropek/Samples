using System;
using System.Globalization;
using System.Windows.Data;
using IronPython.Hosting;
using Microsoft.Scripting;

namespace IronPython.UI.ViewModel
{
	public class IronPythonExpressionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			scope.SetVariable("Value", value);
			engine.CreateScriptSourceFromString(parameter.ToString(), SourceCodeKind.Expression);
			var result = engine.Execute(parameter.ToString(), scope);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

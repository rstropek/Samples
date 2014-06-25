using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Samples
{
	public class TextContainer
	{
		private string text;

		public string Text
		{
			set { text = value; }
			get { return text; }
		}
	}

	public static class Startup
	{
		public static void Main()
		{
			using (var fs = new FileStream(
				Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "HelloWorldRuntime.xaml"),
			  FileMode.Open))
			{
				TextContainer text = (TextContainer)XamlReader.Load(fs);
				Console.WriteLine(text.Text);
			}
		}
	}
};
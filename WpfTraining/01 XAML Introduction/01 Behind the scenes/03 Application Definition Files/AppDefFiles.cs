using System;
using System.Windows;

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

		public override string ToString()
		{
			return Text;
		}
	}

	public partial class SampleApp : Application
	{
		override protected void OnStartup(StartupEventArgs e)
		{
			Console.WriteLine(FindResource("OutputText"));
			Shutdown();
		}
	}
};
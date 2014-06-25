using System;
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
		private static HelloWorldText text = new HelloWorldText();

		public static void Main()
		{
			text.InitializeComponent();
			Console.WriteLine(text.UpperText);
		}
	}
};
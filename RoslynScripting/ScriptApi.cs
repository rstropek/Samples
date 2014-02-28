using System;

namespace RoslynScripting
{
	public class ScriptApi
	{
		private Action<string> print;

		internal ScriptApi(Action<string> print)
		{
			this.print = print;
		}

		public void Print(string format, params string[] args)
		{
			this.print(string.Format(format, args));
		}
	}
}

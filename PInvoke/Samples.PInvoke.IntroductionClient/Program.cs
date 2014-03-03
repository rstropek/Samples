using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	class Program
	{
		// Use the STAThreadAttribute if the used COM components are not thread safe.
		// This is necessary as the CLR otherwise automatically initializes the COM
		// library using CoInitializeEx(..., COINIT_MULTITHREADED).
		[STAThread]
		static void Main(string[] args)
		{
			CalculationFunctions.Run();

			Structures.Run();

			ExportedClass.Run();

			Win32Samples();

			Callbacks.Run();

			StringHandlingSamples();
		}

		private static void StringHandlingSamples()
		{
			StringMarshalling.ExecuteSample();
			StringIntPtrHandling.ExecuteSample();
		}

		#region Example for a Win32 import
		// Rename the MessageBoxW() function to 'DisplayMessage'.
		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW", SetLastError = true)]
		public static extern int DisplayMessage(int hwnd, string text, string caption, int type);

		private static void Win32Samples()
		{
			DisplayMessage(0, "Hello World!", "Greeting", 0);

			DisplayMessage(999, "Hello World!", "Greeting", 0);
			var win32Error = Marshal.GetLastWin32Error();
			Console.WriteLine("Last Win32 Error: {0}", win32Error);
			Console.WriteLine(new Win32Exception(win32Error).Message);
		}
		#endregion
	}
}

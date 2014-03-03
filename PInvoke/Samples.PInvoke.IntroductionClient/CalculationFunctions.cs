using System;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	public static class CalculationFunctions
	{
		// extern "C" PINVOKE_API int AddNumbers(int x, int y);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int AddNumbers(int x, int y);

		// extern "C" PINVOKE_API int AddArray(int x[], int size);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int AddArray(int[] numbers, int size);

		public static void Run()
		{
			Console.WriteLine(AddNumbers(1, 2));

			var source = new[] { 1, 2, 3 };
			Console.WriteLine(AddArray(source, source.Length));
		}

	}
}

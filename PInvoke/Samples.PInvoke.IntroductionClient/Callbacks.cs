using System;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	public static class Callbacks
	{
		// Note that the first sample (CallMeBackToSayHello) uses __stdcall calling convention.
		// The second sample shows how to use _cdecl.

		// extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void CallMeBackToSayHello(Action callback);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void TriangleCallback(Triangle t);

		// extern "C" PINVOKE_API void ReportPythagorasBack(double a, double b, PYTHAGORASCALLBACK callback);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void ReportPythagorasBack(double a, double b, TriangleCallback callback);

		public static void Run()
		{
			CallMeBackToSayHello(() => Console.WriteLine("\tHello from C#"));

			ReportPythagorasBack(1, 2, t => Console.WriteLine(t.c));
		}
	}
}

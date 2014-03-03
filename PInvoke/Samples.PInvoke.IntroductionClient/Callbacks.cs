using System;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	public static class Callbacks
	{
		// Note that the calling convention _cdecl is only used for calling from .NET to
		// C++. The callbacks use __stdcall as shown in Callbacks.h

		// extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void CallMeBackToSayHello(Action callback);

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

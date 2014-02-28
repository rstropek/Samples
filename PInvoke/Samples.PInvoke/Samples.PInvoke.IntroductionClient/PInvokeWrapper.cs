using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Samples.PInvoke.IntroductionClient
{
	public class PInvokeWrapper
	{
		// Note that we need to specify the calling convention here. This is necessary
		// because of changes in the behavior of P/Invoke in .NET 4. See
		// http://msdn.microsoft.com/en-us/library/0htdy0k3.aspx for details.

		#region CalculationFunctions example
		// extern "C" PINVOKE_API int AddNumbers(int x, int y);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int AddNumbers(int x, int y);

		// extern "C" PINVOKE_API int AddArray(int x[], int size);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int AddArray(int[] numbers, int size);
		#endregion

		#region Samples for structs and arrays
		// extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern void DisplayBetterCar(Car2 c);

		// extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern void GiveMeThreeBasicCars(out IntPtr theCars);

		// Helper function to consume imported GiveMeThreeBasicCars function
		public static IEnumerable<Car> GiveMeThreeBasicCarsHelper()
		{
			const int size = 3;
			var result = new List<Car>(size);

			// Pass in an IntPtr as an output parameter.
			IntPtr outArray;
			PInvokeWrapper.GiveMeThreeBasicCars(out outArray);
			try
			{
				// Helper for iterating over array elements
				IntPtr current = outArray;
				for (int i = 0; i < size; i++)
				{
					// Get next car using Marshal.PtrToStructure()
					var car = Marshal.PtrToStructure<Car>(current);
					result.Add(car);

					// Calculate location of next structure using Marshal.SizeOf().
					current = (IntPtr)((int)current + Marshal.SizeOf<Car>());
				}
			}
			finally
			{
				// Free memory for the allocated array.
				Marshal.FreeCoTaskMem(outArray);
			}

			return result;
		}

		// extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern void FillThreeBasicCars([In, Out, MarshalAs(UnmanagedType.LPArray)] CarStruct[] theCars);

		// extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void GiveMeMakes(
			[Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr, SizeParamIndex = 1)] out string[] makes,
			[Out] out int length);
		#endregion

		#region Examples for importing functions using C++ classes
		// extern "C" PINVOKE_API CMiniVan* CreateMiniVan();
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr CreateMiniVan();

		// extern "C" PINVOKE_API void DeleteMiniVan(CMiniVan* obj);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DeleteMiniVan(IntPtr miniVan);

		// extern "C" PINVOKE_API int GetNumberOfSeats(CMiniVan* obj);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetNumberOfSeats(IntPtr miniVan);
		#endregion

		#region Example for a Win32 import
		// Rename the MessageBoxW() function to 'DisplayMessage'.
		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
		public static extern int DisplayMessage(int hwnd, string text, string caption, int type);
		#endregion

		#region Callback examples
		// extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void CallMeBackToSayHello(Action callback);

		public delegate void TriangleCallback(Triangle t);

		// extern "C" PINVOKE_API void ReportPythagorasBack(double a, double b, PYTHAGORASCALLBACK callback);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ReportPythagorasBack(double a, double b, TriangleCallback callback);
		#endregion
	}
}

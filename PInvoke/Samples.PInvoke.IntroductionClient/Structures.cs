using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Samples.PInvoke.IntroductionClient
{
	public static unsafe class Structures
	{
		// .NET type for C++ struct CAR
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private class Car
		{
			public string Make;
			public string Color;
		}

		// .NET type containing another type (see C++ struct BETTERCAR)
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private class BetterCar
		{
			public Car Car = new Car();
			public string PetName;
		}

		// A structure containing char arrays (see C++ struct CARFIXED)
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CarFixed
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string Make;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string Color;
		}

		// Different flavor of CarFixed; this time with fixed array
		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct CarFixed2
		{
			public fixed char Make[256];
			public fixed char Color[256];
		}

		// extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void GiveMeThreeBasicCars(out IntPtr theCars);

		// Helper function to consume imported GiveMeThreeBasicCars function
		private static IEnumerable<Car> GiveMeThreeBasicCarsHelper()
		{
			const int size = 3;
			var result = new List<Car>(size);

			// Pass in an IntPtr as an output parameter.
			IntPtr outArray;
			GiveMeThreeBasicCars(out outArray);
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

		// extern "C" PINVOKE_API void DisplayBetterCar(BETTERCAR* theCar);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void DisplayBetterCar([In] BetterCar c);

		// extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void FillThreeBasicCars([Out, MarshalAs(UnmanagedType.LPArray)] CarFixed[] theCars);

		// Flavor of FillThreeBasicCars, this time with void*
		// extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FillThreeBasicCars")]
		private static extern void FillThreeBasicCars2(void* theCars);

		// Note how we use ArraySubType here to handle BSTRs
		// extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void GiveMeMakes(
			[Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr, SizeParamIndex = 1)] out string[] makes,
			[Out] out int length);

		// Note that the following example does the same as the one above. It is
		// just for demo purposes to show how to manually handle BSTRs that were
		// marshaled as an IntPtr.
		// extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode,
			EntryPoint = "GiveMeMakes")]
		private static extern void GiveMeMakesManual([Out] out IntPtr makes, [Out] out int length);

		public static IEnumerable<string> GiveMeMakesHelper()
		{
			IntPtr unmanagedResult = IntPtr.Zero;
			string[] resultList = null;
			try
			{
				// Note that in this example we get the array of BSTRs via an IntPtr.
				int length = 0;
				GiveMeMakesManual(out unmanagedResult, out length);

				resultList = new string[length];

				// Note how we use unsafe C# pointer arithmetic here
				var currentBstr = (void**)unmanagedResult;
				for (int i = 0; i < length; i++)
				{
					// Get string from BSTR. We have to free the BSTRs because they have
					// been allocated with SysAllocString. Note that this does not free
					// the array of BSTR*, it just frees the BSTRs the array elements refer to.
					var stringResult = Marshal.PtrToStringBSTR((IntPtr)(*currentBstr));
					Marshal.FreeBSTR((IntPtr)(*currentBstr++));
					resultList[i] = stringResult;
				}
			}
			finally
			{
				// Free BSTR array.
				if (unmanagedResult != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(unmanagedResult);
				}
			}

			return resultList;
		}

		public static void Run()
		{
			DisplayBetterCar(new BetterCar()
			{
				Car = new Car() { Color = "Black", Make = "Toyota" },
				PetName = "Blacky"
			});

			foreach (var car in GiveMeThreeBasicCarsHelper())
			{
				Console.WriteLine(car.Make);
			}

			var cars = new CarFixed[3];
			FillThreeBasicCars(cars);
			foreach (var car in cars)
			{
				Console.WriteLine(car.Make);
			}

			unsafe
			{
				// Note how we use pointer arithmetic and fixed buffers here
				fixed (CarFixed2* cars2 = new CarFixed2[3])
				{
					FillThreeBasicCars2(cars2);
					for (int i = 0; i < 3; i++)
					{
						Console.WriteLine(new string(cars2[i].Make));
					}
				}
			}

			var makes = new string[3];
			int length = 0;
			GiveMeMakes(out makes, out length);
			foreach (var make in makes)
			{
				Console.WriteLine(make);
			}

			foreach (var make in GiveMeMakesHelper())
			{
				Console.WriteLine(make);
			}
		}
	}
}

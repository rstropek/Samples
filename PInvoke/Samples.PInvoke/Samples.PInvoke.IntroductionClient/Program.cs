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
			CalculationFunctionExample();

			StructAndArraySamples();

			ClassSamples();

			Win32Samples();

			CallbackSamples();

			StringHandlingSamples();
		}

		private static void StringHandlingSamples()
		{
			StringMarshalling.ExecuteSample();
			StringIntPtrHandling.ExecuteSample();
		}

		private static void CallbackSamples()
		{
			PInvokeWrapper.CallMeBackToSayHello(() => Console.WriteLine("\tHello from C#"));

			PInvokeWrapper.ReportPythagorasBack(1, 2, t => Console.WriteLine(t.c));
		}

		private static void Win32Samples()
		{
			PInvokeWrapper.DisplayMessage(0, "Hello World!", "Greeting", 0);

			PInvokeWrapper.DisplayMessage(999, "Hello World!", "Greeting", 0);
			Console.WriteLine("Last Win32 Error: {0}", Marshal.GetLastWin32Error());
			Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()).Message);
		}

		private static void ClassSamples()
		{
			var miniVan = PInvokeWrapper.CreateMiniVan();
			try
			{
				Console.WriteLine(PInvokeWrapper.GetNumberOfSeats(miniVan));
			}
			finally
			{
				PInvokeWrapper.DeleteMiniVan(miniVan);
			}
		}

		private static void StructAndArraySamples()
		{
			PInvokeWrapper.DisplayBetterCar(new Car2()
			{
				Car = new Car() { Color = "Black", Make = "Toyota" },
				PetName = "Blacky"
			});

			foreach (var car in PInvokeWrapper.GiveMeThreeBasicCarsHelper())
			{
				Console.WriteLine(car.Make);
			}

			var cars = new CarStruct[3];
			PInvokeWrapper.FillThreeBasicCars(cars);
			foreach (var car in cars)
			{
				Console.WriteLine(car.Make);
			}

			var makes = new string[3];
			int length = 0;
			PInvokeWrapper.GiveMeMakes(out makes, out length);
		}

		private static void CalculationFunctionExample()
		{
			Console.WriteLine(PInvokeWrapper.AddNumbers(1, 2));

			var source = new[] { 1, 2, 3 };
			Console.WriteLine(PInvokeWrapper.AddArray(source, source.Length));
		}
	}
}

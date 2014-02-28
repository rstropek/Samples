using System;

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
			Console.WriteLine(PInvokeWrapper.AddNumbers(1, 2));

			var source = new[] { 1, 2, 3 };
			Console.WriteLine(PInvokeWrapper.AddArray(source, source.Length));

			PInvokeWrapper.DisplayBetterCar(new Car2()
				{
					Car = new Car() { Color = "Black", Make = "Toyota" },
					PetName = "Blacky"
				});

			foreach (var car in PInvokeWrapper.GiveMeThreeBasicCarsHelper())
			{
				Console.WriteLine(car.Make);
			}

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
	}
}

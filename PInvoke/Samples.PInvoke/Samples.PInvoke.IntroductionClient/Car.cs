using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	[StructLayout(LayoutKind.Sequential)]
	public class Car
	{
		public Car()
		{
		}

		public Car(Car source)
		{
			this.Make = source.Make;
			this.Color = source.Color;
		}

		public string Make;
		public string Color;
	}

	// A structure containing another structure.
	[StructLayout(LayoutKind.Sequential)]
	public class Car2
	{
		public Car Car = new Car();

		public string PetName;
	}
}

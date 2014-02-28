using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	// Simple structure for C++ struct CAR
	[StructLayout(LayoutKind.Sequential)]
	public class Car
	{
		public string Make;
		public string Color;
	}

	// A structure containing another structure (see C++ struct CAR2)
	[StructLayout(LayoutKind.Sequential)]
	public class Car2
	{
		public Car Car = new Car();

		public string PetName;
	}

	// A structure containing char arrays (see C++ struct CARFIXED)
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct CarStruct
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Make;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Color;
	}
}

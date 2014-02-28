using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Triangle
	{
		public double a;
		public double b;
		public double c;
	}
}

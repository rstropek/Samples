// NOTE THAT THIS SAMPLE IS BASED ON Microsoft Developer Network Library
// AVAILABLE AT http://msdn.microsoft.com/en-us/library/e765dyyy(v=vs.110).aspx

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Samples.PInvoke.IntroductionClient
{
	// Declares a managed structure for each unmanaged structure.
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MyStrStruct
	{
		public string buffer;
		public int size;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct MyStrStruct2
	{
		public string buffer;
		public int size;
	}

	public class StringMarshalling
	{
		// Declares managed prototypes for unmanaged functions.
		[DllImport("PinvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern string TestStringAsResult();

		[DllImport("PinvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void TestStringInStruct(ref MyStrStruct mss);

		[DllImport("PinvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void TestStringInStructAnsi(ref MyStrStruct2 mss);

		public static void ExecuteSample()
		{
			// String as result. 
			string str = TestStringAsResult();
			Console.WriteLine("\nString returned: {0}", str);

			// Initializes buffer and appends something to the end so the whole 
			// buffer is passed to the unmanaged side.
			StringBuilder buffer = new StringBuilder("content", 100);
			buffer.Append((char)0);
			buffer.Append('*', buffer.Capacity - 8);

			MyStrStruct mss;
			mss.buffer = buffer.ToString();
			mss.size = mss.buffer.Length;

			TestStringInStruct(ref mss);
			Console.WriteLine("\nBuffer after Unicode function call: {0}",
				mss.buffer);

			StringBuilder buffer2 = new StringBuilder("content", 100);
			buffer2.Append((char)0);
			buffer2.Append('*', buffer2.Capacity - 8);

			MyStrStruct2 mss2;
			mss2.buffer = buffer2.ToString();
			mss2.size = mss2.buffer.Length;

			TestStringInStructAnsi(ref mss2);
			Console.WriteLine("\nBuffer after Ansi function call: {0}",
				mss2.buffer);
		}
	}
}

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	public static class StringIntPtrHandling
	{
		// Note that GetPrivateProfileSectionNames returns a string with embedded NULL characters.
		// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms724352(v=vs.85).aspx
		// for details.

		[DllImport("kernel32.dll")]
		static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, int nSize, string lpFileName);

		public static void ExecuteSample()
		{
			IntPtr ptr = IntPtr.Zero;
			string s = string.Empty;

			try
			{
				// Allocate a buffer in unmanaged memory
				ptr = Marshal.AllocHGlobal(1024);

				// Call Kernel API
				var numChars = GetPrivateProfileSectionNames(
					ptr,
					1024,
					Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sample.ini"));

				// Copy the buffer into a managed string
				s = Marshal.PtrToStringAnsi(ptr, numChars - 1);
			}
			finally
			{
				// Free the unmanaged buffer
				if (ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(ptr);
				}
			}

			// Display the sections by splitting the string based on NULL characters
			foreach (var section in s.Split('\0'))
			{
				Console.WriteLine(section);
			}
		}
	}
}

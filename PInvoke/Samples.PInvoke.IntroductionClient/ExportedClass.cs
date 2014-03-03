using System;
using System.Runtime.InteropServices;

namespace Samples.PInvoke.IntroductionClient
{
	public static class ExportedClass
	{
		// extern "C" PINVOKE_API CMiniVan* CreateMiniVan();
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr CreateMiniVan();

		// extern "C" PINVOKE_API void DeleteMiniVan(CMiniVan* obj);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void DeleteMiniVan(IntPtr miniVan);

		// extern "C" PINVOKE_API int GetNumberOfSeats(CMiniVan* obj);
		[DllImport("PInvokeIntroduction.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetNumberOfSeats(IntPtr miniVan);

		public static void Run()
		{
			var miniVan = CreateMiniVan();
			try
			{
				Console.WriteLine(GetNumberOfSeats(miniVan));
			}
			finally
			{
				DeleteMiniVan(miniVan);
			}
		}

		// The following class shows how to build a wrapper class using IDisposable
		public class MiniVan : IDisposable
		{
			private IntPtr miniVan;

			public MiniVan()
			{
				this.miniVan = ExportedClass.CreateMiniVan();
			}

			~MiniVan()
			{
				Dispose(false);
			}

			public int NumberOfSeats
			{
				get
				{
					if (this.miniVan == null)
					{
						throw new ObjectDisposedException(this.ToString());
					}

					return ExportedClass.GetNumberOfSeats(this.miniVan);
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected void Dispose(bool disposing)
			{
				if (this.miniVan != null)
				{
					// Release unmanaged resources
					ExportedClass.DeleteMiniVan(this.miniVan);
					this.miniVan = IntPtr.Zero;
				}
			}

		}
	}
}

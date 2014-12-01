using System;
using System.Runtime.InteropServices;

namespace ComDllToRegister
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ProgId("WiXTraining.Calculator")]
	[Guid("BB5C081B-A8ED-4A0D-BE03-809B21A62959")]
    public class SimpleComServer
    {
		public int Add(int x, int y)
		{
			return x + y;
		}

		public int Sub(int x, int y)
		{
			return x - y;
		}
    }
}

using System;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

namespace AppDomainConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var ads = new AppDomainSetup();
			ads.ApplicationBase = Environment.CurrentDirectory;
			var ps = new PermissionSet(PermissionState.None);
			ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

			var ad = AppDomain.CreateDomain("MyDomain", null, ads, ps, null);
			var obj = (ComplexUnsafeMath)ad.CreateInstanceAndUnwrap(
				Assembly.GetAssembly(typeof(ComplexUnsafeMath)).FullName,
				"ComplexUnsafeMath");

			Console.WriteLine(obj.DoUnsafeCalculation(10, 20));
			Console.ReadLine();
		}
	}
}

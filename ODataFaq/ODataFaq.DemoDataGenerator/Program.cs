using ODataFaq.DataModel;
using System;

namespace ODataFaq.DemoDataGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press any key to continue with clearing the database 'ODataFaq' on (localdb)\\v11.0 and filling it with demo data ...");
			Console.ReadKey();

			Console.WriteLine("Please be patient, generating data ...");
			using (var omc = new OrderManagementContext())
			{
				omc.ClearAndFillWithDemoData().Wait();
			}

			Console.WriteLine("Done. Press any key to quit ...");
			Console.ReadKey();
		}
	}
}

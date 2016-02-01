using AdoNetPerfProfiling.Controller;
using AdoNetProfiling.Common;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.IO;
using System.Reflection;
using System.Web.Http;

namespace AdoNetPerfProfiling
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://localhost:12345"))
			{
				Console.WriteLine("Listening on port 12345. Press any key to quit.");
				Console.ReadLine();
			}
		}
	}
}

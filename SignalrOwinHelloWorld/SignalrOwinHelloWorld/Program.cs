using Microsoft.Owin.Hosting;
using System;

namespace SignalrOwinHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start a self-hosting web server
            const string baseUrl = "http://localhost:12345";
            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine($"Server is listening on {baseUrl}");
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Http2Client
{
    class Program
    {
        static string Folder = @"C:\Code\GitHub\Samples\CSharp8\Http2Client\images";
        static readonly Version HttpVersion = new Version(2, 0);
        const int NumberOfTiles = 150;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Folder = args[0];
            }

            Console.WriteLine($"Writing to folder {Folder}");
            Console.WriteLine($"Using version {HttpVersion}");

            // Delete all files in target folder
            foreach (var file in Directory.EnumerateFiles(Folder, "*.png")) { File.Delete(file); }

            // Enable TLS versions
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Create HttpClient with correct version
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://1153288396.rsc.cdn77.org/http2/tiles_final/"),
                DefaultRequestVersion = HttpVersion
            };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

            ExecuteTest(client);
        }

        private static void ExecuteTest(HttpClient client)
        {
            var watch = Stopwatch.StartNew();

            Console.WriteLine("Downloading tiles...");
            Parallel.For(0, NumberOfTiles, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (i) =>
                {
                    var response = client.GetAsync($"tile_{i}.png").Result;
                    response.EnsureSuccessStatusCode();
                    File.WriteAllBytes(Path.Combine(Folder, $"tile_{i}.png"), response.Content.ReadAsByteArrayAsync().Result);
                });

            Console.WriteLine(watch.Elapsed);
        }
    }
}

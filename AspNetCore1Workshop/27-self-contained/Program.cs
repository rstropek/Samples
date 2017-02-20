using Newtonsoft.Json;
using System;
using System.IO;

namespace _27_self_contained
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("greet.json"))
            {
                var greetingJson = File.ReadAllText("greet.json");
                dynamic greeting = JsonConvert.DeserializeObject(greetingJson);
                Console.WriteLine(greeting.hello);
            }
        }
    }
}

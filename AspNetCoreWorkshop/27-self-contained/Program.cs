using Newtonsoft.Json;
using System;
using System.IO;

namespace SelfContained
{
    class Program
    {
        static void Main()
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

using CoreClassLibrary;
using System;

namespace OldFullFrameworkClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const string content = @"{ ""name"": ""Superman"" }";
            var result = JsonHelper.Parse(content);
            Console.WriteLine(result.ToString());
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;

namespace InterfaceImplementation
{
    interface ISimpleFileAbstraction
    {
        Task<string> ReadAllText(StreamReader reader);
    }

    interface IFileAbstraction
    {
        Task<string> ReadAllText(StreamReader reader);

        Task<string> ReadAllText(string fileName) => ReadAllText(new StreamReader(fileName));
    }

    class DefaultFileAbstraction : IFileAbstraction, ISimpleFileAbstraction
    {
        public Task<string> ReadAllText(StreamReader reader)
        {
            return reader.ReadToEndAsync();
        }
    }

    class Program
    {
        static async Task Main()
        {
            IFileAbstraction file = new DefaultFileAbstraction();
            Console.WriteLine(await file.ReadAllText(@"c:\temp\demo.txt"));
        }
    }
}

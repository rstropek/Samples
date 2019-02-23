using System;
using System.IO;
using System.Threading.Tasks;

namespace InterfaceImplementation
{
    interface IFileAbstraction
    {
        Task<string> ReadAllText(StreamReader reader);

        Task<string> ReadAllText(string fileName) => ReadAllText(new StreamReader(fileName));
    }

    class Program
    {
        static void Main()
        {

        }
    }
}

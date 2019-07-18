using System;

// Learn more about Target Frameworks at https://docs.microsoft.com/en-us/dotnet/standard/frameworks

namespace MultiTarget
{
    public static class Demo
    {
        public static void SayHello()
        {
#if NET48
            Console.WriteLine("Hello .NET Framework!");
#else
            Console.WriteLine("Hello .NET Standard!");
#endif
        }
    }
}

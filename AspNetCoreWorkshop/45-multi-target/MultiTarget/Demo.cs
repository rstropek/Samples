using System;

namespace MultiTarget
{
    public static class Demo
    {
        public static void SayHello()
        {
#if NET452
            Console.WriteLine("Hello .NET Framework!");
#else
            Console.WriteLine("Hello .NET Standard!");
#endif
        }
    }
}

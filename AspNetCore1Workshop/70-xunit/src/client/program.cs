using System;
using Library;

namespace myApp
{
    public class Program
    {
        public void Main(string[] args)
        {
            Console.WriteLine($"Add: {Library.Math.Add(1, 2)}");
            Console.WriteLine($"Subtract: {Library.Math.Subtract(1, 2)}");
        }
    }
}

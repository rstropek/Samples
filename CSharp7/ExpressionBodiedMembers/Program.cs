using System;

namespace ExpressionBodiedMembers
{
    class Pet
    {
        private string name;

        // Note expression-bodied constructor. Finalizers work similarly.
        public Pet(string name) => Name = name;

        // Note expression-bodied property.
        public string Name
        {
            get => name;
            set => name = value ?? "No name";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        // Note throw expression.
        static int evenNumber = 2;
        static void SetEvenNumber(int n) => 
            evenNumber = (n % 2 == 0) ? n : throw new ArgumentException();

        static string someString = "abc";
        static void SetNotNullString(string s) => 
            someString = s ?? throw new ArgumentException();
    }
}
using System;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members

namespace ExpressionBodiedMembers
{
    // C# 7 now understands much more expression-bodied members.
    // Read more at http://bit.ly/cs-expr-bodied

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
        static void Main()
        {
            Console.WriteLine("Hello World!");
        }


        // Note throw expression and (recap) `nameof`
        private static int evenNumber = 2;
        private static void SetEvenNumber(int n) =>
            evenNumber = (n % 2 == 0) ? n : throw new ArgumentException(nameof(n));

        private static string someString = "abc";
        private static void SetNotNullString(string s) => 
            someString = s ?? throw new ArgumentNullException(nameof(s));
    }
}
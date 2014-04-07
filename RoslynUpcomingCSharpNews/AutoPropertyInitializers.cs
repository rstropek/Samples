using System;

namespace RoslynUpcomingCSharpNews
{
    public class Product_OldStyleWithField
    {
        // Note that we have to care for the private field
        //      ourselves.
        private readonly Guid id = Guid.NewGuid();
        public Guid Id { get { return this.id; } }
    }

    public class Product_AutoProperty
    {
        public Product_AutoProperty()
        {
            this.Id = Guid.NewGuid();
        }

        // Note that we cannot define "Id" as "readonly" as
        // read-only properties are not supported in C#.
        public Guid Id { get; private set; }
    }

    public class Product_AutoPropertyInitializer
    {
        // Note that "Id" is a getter-only property -> immutable :-)
        public Guid Id { get; } = Guid.NewGuid();
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RoslynUpcomingCSharpNews
{
    // Class with "primary constructor"
    public class Product(
        public int Id,
        string ProductName,
        private bool Available = true)
    {
        // "Id" automatically becomes a public field because 
        //      of the primary constructor.

        // Primary constructor parameter used for initialization.
        public string Name { get; } = ProductName;

        // "Available" automatically becomes a private field
        //             because of the primary constructor.
        public void Buy()
        {
            if (!this.Available)
            {
                throw new InvalidOperationException("Not available");
            }

            // Note that the following line would not work because
            //      primary constructor parameters have to be captured
            //      (with e.g. "public" or "private") or used to 
            //      initialize members.
            // Debug.WriteLine("Buying product {0}", this.ProductName);
        }

        // The following constructor would not work because
        //     it has to provide parameters for primary constructor.
        // public Product() { }

        // This constructor works because it contains a call to "this"
        public Product() : this(0, null) { }
    }

    // Note how inheritance works with primary constructors
    public class OldProduct(int Id, string Name) : Product(Id, Name, false)
    {
    }

    [TestClass]
    public class PrimaryConstructorsTests
    {
        [TestMethod]
        public void TestClassWithPrimaryConstructor()
        {
            // Note the use of the primary constructor here
            var p = new Product(1, "Bike");

            // Note that the Id field is available on p because of the
            // "public" keyword in the primary constructor.
            Assert.AreEqual(1, p.Id);
        }
    }

}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RoslynUpcomingCSharpNews
{
    [TestClass]
    public class DeclarationExpressions
    {
        [TestMethod]
        public void CompareDeclarationExpressions()
        {
            var products = new[]
            {
                new { Id = 1, Name = "Bike", PriceAsString = "1000" },
                new { Id = 2, Name = "Car", PriceAsString = "20000" },
                new { Id = 3, Name = "Plane", PriceAsString = "A lot of money!" }
            };

            // Our goal: Calculate total price and ignore prices that 
            //           contain invalid characters.

            // Calculate total price (old style)
            var totalPrice = products
                .Sum(p =>
                {
                    int price;
                    if (Int32.TryParse(p.PriceAsString, out price))
                    {
                        return price;
                    }

                    return 0;
                });
            Assert.AreEqual(21000, totalPrice);

            // Calculate total price (new style)
            totalPrice = products
                .Sum(p => Int32.TryParse(p.PriceAsString, out var price) ? price : 0);
            Assert.AreEqual(21000, totalPrice);

            // Declare variable in expression.
            int x = 1;
            int y = 1;
            if (var areEqual = x != y)
            {
                Assert.Fail();
            }
        }
    }
}

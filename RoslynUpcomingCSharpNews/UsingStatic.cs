using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MyOtherNamespace
{
    // Note that "CurrencyConverter" is a static class.
    public static class CurrencyConverter
    {
        public static decimal ConvertToEuro(string sourceCurrency, 
            decimal price)
        {
            if (sourceCurrency == "USD")
            {
                return price * 0.73m;
            }

            throw new ArgumentException(
                "Unknown source currency", 
                "sourceCurrency");
        }
    }
}

namespace RoslynUpcomingCSharpNews
{
    // Note that the "using" refers to a class instead a namespace.
    using MyOtherNamespace.CurrencyConverter;

    [TestClass]
    public class UsingStatic
    {
        [TestMethod]
        public void TestClassWithPrimaryConstructor()
        {
            const decimal priceInUsd = 100m;

            // Note how we call CurrencyConverter.ConvertToEuro
            // without class name.
            var priceInEuro = ConvertToEuro("USD", 100);

            Assert.IsTrue(priceInEuro < priceInUsd);
        }
    }
}
